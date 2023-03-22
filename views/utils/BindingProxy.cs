﻿using System.Windows;

namespace ReferenceConfigurator.views.utils {
    public class BindingProxy : Freezable {
        public static readonly DependencyProperty DataProperty =
           DependencyProperty.Register("Data", typeof(object),
              typeof(BindingProxy));

        public object Data {
            get { return GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        #region Overrides of Freezable

        protected override Freezable CreateInstanceCore() {
            return new BindingProxy();
        }

        #endregion
    }
}
