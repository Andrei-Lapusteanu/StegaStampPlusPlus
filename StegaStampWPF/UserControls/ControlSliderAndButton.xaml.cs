using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StegaStampWPF.UserControls
{
    /// <summary>
    /// Interaction logic for ControlSliderAndButton.xaml
    /// </summary>
    public partial class ControlSliderAndButton : UserControl
    {
        public event EventHandler UserControlClicked;

        public ControlSliderAndButton()
        {
            InitializeComponent();
        }
        private void ControlButton_Click(object sender, RoutedEventArgs e)
        {
            UserControlClicked?.Invoke(this, EventArgs.Empty);
        }

        public string TextBlockTitleBar
        {
            get { return textBlockTitleBar.Text; }
            set { textBlockTitleBar.Text = value; }
        }

        public double SliderValue
        {
            get { return sliderControl.Value; }
            set { sliderControl.Value = value; }
        }

        public double SliderMin
        {
            get { return sliderControl.Minimum; }
            set { sliderControl.Minimum = value; }
        }

        public double SliderMax
        {
            get { return sliderControl.Maximum; }
            set { sliderControl.Maximum = value; }
        }

        public double SliderSmallChange
        {
            get { return sliderControl.SmallChange; }
            set { sliderControl.SmallChange = value; }
        }

        public double SliderLargeChange
        {
            get { return sliderControl.LargeChange; }
            set { sliderControl.LargeChange = value; }
        }

        public double SliderTickFrequency
        {
            get { return sliderControl.TickFrequency; }
            set { sliderControl.TickFrequency = value; }
        }

        public string TextBlockSliderMin
        {
            get { return textBlockSliderMin.Text; }
            set { textBlockSliderMin.Text = value; }
        }

        public string TextBlockSliderMax
        {
            get { return textBlockSliderMax.Text; }
            set { textBlockSliderMax.Text = value; }
        }

        public string TextBlockSliderVal
        {
            get { return textBlockSliderVal.Text; }
            set { textBlockSliderVal.Text = value; }
        }

        public TickPlacement SetTickPlacement
        {
            get { return sliderControl.TickPlacement; }
            set { sliderControl.TickPlacement = value; }
        }

        private void sliderControl_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            TextBlockSliderVal = sliderControl.Value.ToString("0");
        }
    }
}
