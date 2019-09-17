using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Threading;
using System.Threading.Tasks;

namespace MazeRuner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Maze MazeMatrix;
        System.Timers.Timer Clock;
        Stack<ValueTuple<int, int>> Path;
        bool isCancel;
        bool isPause;
        public MainWindow()
        {
            InitializeComponent();
            Row.Text = "4";
            Column.Text = "5";
            Path = new Stack<(int, int)>();
            Clock = new System.Timers.Timer();
            Speed.Text = "1000";
            Clock.Elapsed += (s, e) => Clock.Stop();
            Clock.Elapsed += TimeElapse;
            Clock.Start();
            isPause = false;
            isCancel = false;
        }

        void TimeElapse(object sender, System.Timers.ElapsedEventArgs e)
        {
            Monitor.Enter("ClockLock");
            if (!isPause)
                Monitor.Pulse("ClockLock");
            Monitor.Wait("ClockLock");
            Monitor.Exit("ClockLock");
            Clock.Start();
        }

        void FindPath()
        {
            bool is8dir = false;
            Dispatcher.Invoke(() =>
            {
                is8dir = RBIs8Dir.IsChecked.Value;
                for (int i = 0; i < MazeMatrix.Row; i++)
                    for (int j = 0; j < MazeMatrix.Column; j++)
                        if (MazeBlocks[i, j] == Brushes.Black)
                        {
                            MazeMatrix[i, j] = new Block();
                        }
                        else if (MazeMatrix[i, j].IsWall)
                        {
                            MazeMatrix[i, j] = new Block(is8dir);
                            MazeBlocks[i, j] = Brushes.GhostWhite;
                        }
                        else
                        {
                            MazeMatrix[i, j] = new Block(is8dir);
                            MazeBlocks[i, j] = Brushes.GhostWhite;
                        }
            });
            Monitor.Enter("ClockLock");
            Block temp, temp2;
            Path.Clear();
            Path.Push((0, 0));
            Dispatcher.Invoke(() => MazeBlocks[0, 0] = Brushes.Yellow);
            while (Path.Peek() != (MazeMatrix.Row - 1, MazeMatrix.Column - 1))
            {
                temp = MazeMatrix[Path.Peek().Item1, Path.Peek().Item2];
                while (true)
                {
                    temp.NextDir();
                    if (temp.Checked) break;
                    temp2 = MazeMatrix[Path.Peek().Item1 + temp.NextDir_w, Path.Peek().Item2 + temp.NextDir_H];
                    if (is8dir && temp.NextDir_w != 0 && temp.NextDir_H != 0)
                    {
                        if (MazeMatrix[Path.Peek().Item1 + temp.NextDir_w, Path.Peek().Item2].IsWall
                            && MazeMatrix[Path.Peek().Item1, Path.Peek().Item2 + temp.NextDir_H].IsWall) continue;
                    }
                    if (!temp2.IsWall && !temp2.Seen) break;
                }
                if (temp.Checked)
                {
                    if (isCancel)
                    {
                        isCancel = false;
                        Monitor.Exit("ClockLock");
                        return;
                    }
                    Monitor.Pulse("ClockLock");
                    Monitor.Wait("ClockLock");
                    Dispatcher.Invoke(() => MazeBlocks[Path.Peek().Item1, Path.Peek().Item2] = Brushes.Red);
                    Path.Pop();
                    if (Path.Count == 0) break;
                    continue;
                }
                if (isCancel)
                {
                    isCancel = false;
                    Monitor.Exit("ClockLock");
                    return;
                }
                Monitor.Pulse("ClockLock");
                Monitor.Wait("ClockLock");
                Path.Push((Path.Peek().Item1 + temp.NextDir_w, Path.Peek().Item2 + temp.NextDir_H));
                Dispatcher.Invoke(() => MazeBlocks[Path.Peek().Item1, Path.Peek().Item2] = Brushes.Yellow);
            }
            Monitor.Exit("ClockLock");
            foreach (var block in Path)
                Dispatcher.Invoke(() => MazeBlocks[block.Item1, block.Item2] = Brushes.Green);
        }

        private void Change_MazeSize(object sender, TextChangedEventArgs e)
        {
            int row, column;
            int.TryParse(Row.Text, out row);
            int.TryParse(Column.Text, out column);
            if (row == 0) row = 1;
            if (column == 0) column = 1;
            MazeMatrix = new Maze(row, column, RBIs8Dir.IsChecked.Value);
            MazeBlocks.Row = row;
            MazeBlocks.Column = column;
        }

        private void Go_Click(object sender, RoutedEventArgs e)
        {
            Cancel.Visibility = Visibility.Visible;
            Pause.Visibility = Visibility.Visible;
            Go.IsEnabled = false;
            Reset.IsEnabled = false;
            MazeBlocks.IsEnabled = false;
            Row.IsEnabled = false;
            Column.IsEnabled = false;
            var ThrPath = new Task(FindPath);
            ThrPath.ContinueWith((dd) =>
            {
                Dispatcher.Invoke(() =>
                {
                    Pause.Visibility = Visibility.Hidden;
                    Cancel.Visibility = Visibility.Hidden;
                    Go.IsEnabled = Reset.IsEnabled = true;
                    MazeBlocks.IsEnabled = true;
                    Row.IsEnabled = true;
                    Column.IsEnabled = true;
                });
            });
            ThrPath.Start();
            Sts.Content = "Running";
        }

        private void Speed_TextChanged(object sender, TextChangedEventArgs e)
        {
            double interval;
            double.TryParse(Speed.Text, out interval);
            Clock.Interval = interval != 0 ? interval : 1;
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < MazeMatrix.Row; i++)
                for (int j = 0; j < MazeMatrix.Column; j++)
                    if ((MazeMatrix[i, j].Seen || (i == MazeMatrix.Row - 1 && j == MazeMatrix.Column - 1))&&MazeBlocks[i,j]!=Brushes.Black)
                    {
                        MazeMatrix[i, j] = new Block(RBIs8Dir.IsChecked.Value);
                        MazeBlocks[i, j] = Brushes.GhostWhite;
                    }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            isCancel = true;
            Sts.Content = "";
            if(isPause)
            {
                isPause = false;
                Pause.Content = "Pause";
                Monitor.Enter("ClockLock");
                Monitor.Pulse("ClockLock");
                Monitor.Exit("ClockLock");
            }
            (e.OriginalSource as Button).Visibility = Visibility.Hidden;
        }

        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            if (isPause)
            {
                Sts.Content = "Running";
                isPause = false;
                Pause.Content = "Pause";
                Monitor.Enter("ClockLock");
                Monitor.Pulse("ClockLock");
                Monitor.Exit("ClockLock");
            }
            else
            {
                Sts.Content = "Pause";
                isPause = true;
                Pause.Content = "Continue";
            }
        }
    }
}
