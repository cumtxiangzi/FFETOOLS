﻿#pragma checksum "..\..\WellPoint.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "0E97E63FC100199BA96A2CEA96244C0DFC2DC24D"
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
    /// WellPointWindow
    /// </summary>
    public partial class WellPointWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 16 "..\..\WellPoint.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button CollectButton;
        
        #line default
        #line hidden
        
        
        #line 17 "..\..\WellPoint.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button OPButton;
        
        #line default
        #line hidden
        
        
        #line 18 "..\..\WellPoint.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ExitButton;
        
        #line default
        #line hidden
        
        
        #line 19 "..\..\WellPoint.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button CreatPipeButton;
        
        #line default
        #line hidden
        
        
        #line 20 "..\..\WellPoint.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button InputButton;
        
        #line default
        #line hidden
        
        
        #line 21 "..\..\WellPoint.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button WellButton;
        
        #line default
        #line hidden
        
        
        #line 23 "..\..\WellPoint.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid PS_List;
        
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
            System.Uri resourceLocater = new System.Uri("/OutdoorPipe;component/wellpoint.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\WellPoint.xaml"
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
            
            #line 8 "..\..\WellPoint.xaml"
            ((FFETOOLS.WellPointWindow)(target)).Loaded += new System.Windows.RoutedEventHandler(this.Window_Loaded);
            
            #line default
            #line hidden
            return;
            case 2:
            this.CollectButton = ((System.Windows.Controls.Button)(target));
            
            #line 16 "..\..\WellPoint.xaml"
            this.CollectButton.Click += new System.Windows.RoutedEventHandler(this.CollectButton_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.OPButton = ((System.Windows.Controls.Button)(target));
            
            #line 17 "..\..\WellPoint.xaml"
            this.OPButton.Click += new System.Windows.RoutedEventHandler(this.OPButton_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.ExitButton = ((System.Windows.Controls.Button)(target));
            
            #line 18 "..\..\WellPoint.xaml"
            this.ExitButton.Click += new System.Windows.RoutedEventHandler(this.ExitButton_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.CreatPipeButton = ((System.Windows.Controls.Button)(target));
            
            #line 19 "..\..\WellPoint.xaml"
            this.CreatPipeButton.Click += new System.Windows.RoutedEventHandler(this.CreatPipeButton_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.InputButton = ((System.Windows.Controls.Button)(target));
            
            #line 20 "..\..\WellPoint.xaml"
            this.InputButton.Click += new System.Windows.RoutedEventHandler(this.InputButton_Click);
            
            #line default
            #line hidden
            return;
            case 7:
            this.WellButton = ((System.Windows.Controls.Button)(target));
            
            #line 21 "..\..\WellPoint.xaml"
            this.WellButton.Click += new System.Windows.RoutedEventHandler(this.WellButton_Click);
            
            #line default
            #line hidden
            return;
            case 8:
            this.PS_List = ((System.Windows.Controls.DataGrid)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

