﻿#pragma checksum "..\..\..\FamilyManager\FamilyManager.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "D58B382144C02B46E55840EAEF86F17CA3959F19A6788C07B34B1C2646CF3F89"
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

using FFETOOLS;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace FFETOOLS {
    
    
    /// <summary>
    /// FamilyManagerWindow
    /// </summary>
    public partial class FamilyManagerWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 15 "..\..\..\FamilyManager\FamilyManager.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TreeView FamilyTreeList;
        
        #line default
        #line hidden
        
        
        #line 17 "..\..\..\FamilyManager\FamilyManager.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Btn_Load;
        
        #line default
        #line hidden
        
        
        #line 18 "..\..\..\FamilyManager\FamilyManager.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Btn_Cancel;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/BatchTools;component/familymanager/familymanager.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\FamilyManager\FamilyManager.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.FamilyTreeList = ((System.Windows.Controls.TreeView)(target));
            
            #line 15 "..\..\..\FamilyManager\FamilyManager.xaml"
            this.FamilyTreeList.Loaded += new System.Windows.RoutedEventHandler(this.FamilyTreeList_Loaded);
            
            #line default
            #line hidden
            
            #line 15 "..\..\..\FamilyManager\FamilyManager.xaml"
            this.FamilyTreeList.AddHandler(System.Windows.Controls.TreeViewItem.ExpandedEvent, new System.Windows.RoutedEventHandler(this.FamilyTreeList_Expanded));
            
            #line default
            #line hidden
            
            #line 15 "..\..\..\FamilyManager\FamilyManager.xaml"
            this.FamilyTreeList.AddHandler(System.Windows.Controls.TreeViewItem.SelectedEvent, new System.Windows.RoutedEventHandler(this.FamilyTreeList_Selected));
            
            #line default
            #line hidden
            return;
            case 2:
            this.Btn_Load = ((System.Windows.Controls.Button)(target));
            
            #line 17 "..\..\..\FamilyManager\FamilyManager.xaml"
            this.Btn_Load.Click += new System.Windows.RoutedEventHandler(this.Btn_Load_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.Btn_Cancel = ((System.Windows.Controls.Button)(target));
            
            #line 18 "..\..\..\FamilyManager\FamilyManager.xaml"
            this.Btn_Cancel.Click += new System.Windows.RoutedEventHandler(this.Btn_Cancel_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

