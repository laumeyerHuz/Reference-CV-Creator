using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReferenceConfigurator.models {
    public class DataGridCheckBoxModel : ObservableObject {
        public string Name { get; set; }

        private bool _checked1;
        public bool IsChecked1 {
            get => _checked1;
            set => SetProperty(ref _checked1, value);
        }

        private bool _checked2;
        public bool IsChecked2 {
            get => _checked2;
            set => SetProperty(ref _checked2, value);
        }

        private bool _checked3;
        public bool IsChecked3 {
            get => _checked3;
            set => SetProperty(ref _checked3, value);
        }

        public DataGridCheckBoxModel() {

        }

        public DataGridCheckBoxModel(string name, bool value) {
            this.Name = name;
            IsChecked1 = value;
            IsChecked2 = value;
            IsChecked3 = value;
        }
        public DataGridCheckBoxModel(string name, bool IsChecked1, bool IsChecked2, bool IsChecked3) {
            this.Name = name;
            this.IsChecked1 = IsChecked1;
            this.IsChecked2 = IsChecked2;
            this.IsChecked3 = IsChecked3;
        }
    }
}
