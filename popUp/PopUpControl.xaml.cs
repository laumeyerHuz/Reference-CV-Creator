using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ReferenceConfigurator.views;
using Lucene.Net.Util.Automaton;
using CommunityToolkit.Mvvm.Input;

namespace ReferenceConfigurator.popUp {
    /// <summary>
    /// Interaction logic for PopUpControl.xaml
    /// </summary>
    public partial class PopUpControl : UserControl{

        public PopUpControl() {
            InitializeComponent();
            this.DataContext = new PopUpViewModel();
        }
    }

    
}
