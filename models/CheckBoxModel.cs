using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReferenceConfigurator.models {
    public class CheckBoxModel : ObservableObject {
        public string Name { get; set; }

        private bool _checked;
        public bool IsChecked { 
            get => _checked; 
            set => SetProperty(ref _checked, value);
        }
    }
}
