using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace D2DScreenMenuExample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Storyboard storyboard;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();

            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            storyboard = new Storyboard();
            storyboard.RepeatBehavior = RepeatBehavior.Forever;
            var doubleanimtion = new DoubleAnimation(0, 360, new Duration(TimeSpan.FromSeconds(15)));
            storyboard.Children.Add(doubleanimtion);
            Storyboard.SetTarget(doubleanimtion, target);
            Storyboard.SetTargetProperty(doubleanimtion, new PropertyPath("Transform.Angle"));
        }

        private void Button2D_Clicked2D(object sender, HelixToolkit.Wpf.SharpDX.Elements2D.Mouse2DEventArgs e)
        {
            storyboard.Begin();
        }

        private void Button2D_Clicked2D_1(object sender, HelixToolkit.Wpf.SharpDX.Elements2D.Mouse2DEventArgs e)
        {
            storyboard.Stop();
        }
    }
}
