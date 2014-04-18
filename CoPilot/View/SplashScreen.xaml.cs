using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media.Animation;

namespace CoPilot.CoPilot.View
{
    public partial class SplashScreen : UserControl
    {
        /// <summary>
        /// Splash screen
        /// </summary>
        public SplashScreen()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Animate
        /// </summary>
        public void Animate(int timeout)
        {
            //hide
            this.Progress.Visibility = Visibility.Collapsed;

            //animate
            DoubleAnimation myDoubleAnimation = new DoubleAnimation();
            myDoubleAnimation.From = 0;
            myDoubleAnimation.To = 90;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(timeout));          

            Storyboard.SetTarget(myDoubleAnimation, this.Screen);
            Storyboard.SetTargetProperty(myDoubleAnimation, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.Rotation)"));

            Storyboard myMovementStoryboard = new Storyboard();
            myMovementStoryboard.Children.Add(myDoubleAnimation);
            myMovementStoryboard.Begin();
        }
    }
}
