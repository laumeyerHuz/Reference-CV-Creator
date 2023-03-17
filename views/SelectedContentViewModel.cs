using Microsoft.Graph;
using ReferenceConfigurator.models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ReferenceConfigurator.views {
    public abstract class SelectedContentViewModel : ViewModelBase{

        
        public abstract void addReference(ReferenceModel selected);
        public abstract void removeReference(ReferenceModel selected);

        public abstract List<ReferenceModel> getReferenceList();
    }
}
