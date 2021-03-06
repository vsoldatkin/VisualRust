//*********************************************************//
//    Copyright (c) Microsoft. All rights reserved.
//    
//    Apache 2.0 License
//    
//    You may obtain a copy of the License at
//    http://www.apache.org/licenses/LICENSE-2.0
//    
//    Unless required by applicable law or agreed to in writing, software 
//    distributed under the License is distributed on an "AS IS" BASIS, 
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or 
//    implied. See the License for the specific language governing 
//    permissions and limitations under the License.
//
//*********************************************************//

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell.Interop;

namespace Microsoft.VisualStudioTools.Project {
    #region structures
    [StructLayoutAttribute(LayoutKind.Sequential)]
    internal struct _DROPFILES {
        public Int32 pFiles;
        public Int32 X;
        public Int32 Y;
        public Int32 fNC;
        public Int32 fWide;
    }
    #endregion

    #region enums


    /// <summary>
    /// Defines the currect state of a property page.
    /// </summary>
    [Flags]
    public enum PropPageStatus {

        Dirty = 0x1,

        Validate = 0x2,

        Clean = 0x4
    }

    /// <summary>
    /// Defines the status of the command being queried
    /// </summary>
    [Flags]
    public enum QueryStatusResult {
        /// <summary>
        /// The command is not supported.
        /// </summary>
        NOTSUPPORTED = 0,

        /// <summary>
        /// The command is supported
        /// </summary>
        SUPPORTED = 1,

        /// <summary>
        /// The command is enabled
        /// </summary>
        ENABLED = 2,

        /// <summary>
        /// The command is toggled on
        /// </summary>
        LATCHED = 4,

        /// <summary>
        /// The command is toggled off (the opposite of LATCHED).
        /// </summary>
        NINCHED = 8,

        /// <summary>
        /// The command is invisible.
        /// </summary>
        INVISIBLE = 16
    }

    /// <summary>
    /// Defines the type of item to be added to the hierarchy.
    /// </summary>
    public enum HierarchyAddType {
        AddNewItem,
        AddExistingItem
    }

    /// <summary>
    /// Defines the component from which a command was issued.
    /// </summary>
    public enum CommandOrigin {
        UiHierarchy,
        OleCommandTarget
    }

    /// <summary>
    /// Defines the current status of the build process.
    /// </summary>
    public enum MSBuildResult {
        /// <summary>
        /// The build is currently suspended.
        /// </summary>
        Suspended,

        /// <summary>
        /// The build has been restarted.
        /// </summary>
        Resumed,

        /// <summary>
        /// The build failed.
        /// </summary>
        Failed,

        /// <summary>
        /// The build was successful.
        /// </summary>
        Successful,
    }

    /// <summary>
    /// Defines the type of action to be taken in showing the window frame.
    /// </summary>
    public enum WindowFrameShowAction {
        DoNotShow,
        Show,
        ShowNoActivate,
        Hide,
    }

    /// <summary>
    /// Defines drop types
    /// </summary>
    internal enum DropDataType {
        None,
        Shell,
        VsStg,
        VsRef
    }

    /// <summary>
    /// Used by the hierarchy node to decide which element to redraw.
    /// </summary>
    [Flags]
    public enum UIHierarchyElement {
        None = 0,

        /// <summary>
        /// This will be translated to VSHPROPID_IconIndex
        /// </summary>
        Icon = 1,

        /// <summary>
        /// This will be translated to VSHPROPID_StateIconIndex
        /// </summary>
        SccState = 2,

        /// <summary>
        /// This will be translated to VSHPROPID_Caption
        /// </summary>
        Caption = 4,

        /// <summary>
        /// This will be translated to VSHPROPID_OverlayIconIndex
        /// </summary>
        OverlayIcon = 8
    }

    /// <summary>
    /// Defines the global propeties used by the msbuild project.
    /// </summary>
    public enum GlobalProperty {
        /// <summary>
        /// Property specifying that we are building inside VS.
        /// </summary>
        BuildingInsideVisualStudio,

        /// <summary>
        /// The VS installation directory. This is the same as the $(DevEnvDir) macro.
        /// </summary>
        DevEnvDir,

        /// <summary>
        /// The name of the solution the project is created. This is the same as the $(SolutionName) macro.
        /// </summary>
        SolutionName,

        /// <summary>
        /// The file name of the solution. This is the same as $(SolutionFileName) macro.
        /// </summary>
        SolutionFileName,

        /// <summary>
        /// The full path of the solution. This is the same as the $(SolutionPath) macro.
        /// </summary>
        SolutionPath,

        /// <summary>
        /// The directory of the solution. This is the same as the $(SolutionDir) macro.
        /// </summary>               
        SolutionDir,

        /// <summary>
        /// The extension of teh directory. This is the same as the $(SolutionExt) macro.
        /// </summary>
        SolutionExt,

        /// <summary>
        /// The fxcop installation directory.
        /// </summary>
        FxCopDir,

        /// <summary>
        /// The ResolvedNonMSBuildProjectOutputs msbuild property
        /// </summary>
        VSIDEResolvedNonMSBuildProjectOutputs,

        /// <summary>
        /// The Configuartion property.
        /// </summary>
        Configuration,

        /// <summary>
        /// The platform property.
        /// </summary>
        Platform,

        /// <summary>
        /// The RunCodeAnalysisOnce property
        /// </summary>
        RunCodeAnalysisOnce,

        /// <summary>
        /// The VisualStudioStyleErrors property
        /// </summary>
        VisualStudioStyleErrors,
    }
    #endregion

    public class AfterProjectFileOpenedEventArgs : EventArgs {

    }

    public class BeforeProjectFileClosedEventArgs : EventArgs {
        #region fields
        private bool _removed;
        private IVsHierarchy _hierarchy;
        #endregion

        #region properties
        /// <summary>
        /// true if the project was removed from the solution before the solution was closed. false if the project was removed from the solution while the solution was being closed.
        /// </summary>
        internal bool Removed {
            get { return _removed; }
        }

        internal IVsHierarchy Hierarchy {
            get {
                return _hierarchy;
            }
        }

        #endregion

        #region ctor
        internal BeforeProjectFileClosedEventArgs(IVsHierarchy hierarchy, bool removed) {
            this._removed = removed;
            _hierarchy = hierarchy;
        }
        #endregion
    }

    /// <summary>
    /// Argument of the event raised when a project property is changed.
    /// </summary>
    public class ProjectPropertyChangedArgs : EventArgs {
        private string propertyName;
        private string oldValue;
        private string newValue;

        internal ProjectPropertyChangedArgs(string propertyName, string oldValue, string newValue) {
            this.propertyName = propertyName;
            this.oldValue = oldValue;
            this.newValue = newValue;
        }

        public string NewValue {
            get { return newValue; }
        }

        public string OldValue {
            get { return oldValue; }
        }

        public string PropertyName {
            get { return propertyName; }
        }
    }

    /// <summary>
    /// This class is used for the events raised by a HierarchyNode object.
    /// </summary>
    internal class HierarchyNodeEventArgs : EventArgs {
        private HierarchyNode child;

        internal HierarchyNodeEventArgs(HierarchyNode child) {
            this.child = child;
        }

        public HierarchyNode Child {
            get { return this.child; }
        }
    }

    /// <summary>
    /// Event args class for triggering file change event arguments.
    /// </summary>
    public class FileChangedOnDiskEventArgs : EventArgs {
        #region Private fields
        /// <summary>
        /// File name that was changed on disk.
        /// </summary>
        private string fileName;

        /// <summary>
        /// The item ide of the file that has changed.
        /// </summary>
        private uint itemID;

        /// <summary>
        /// The reason the file has changed on disk.
        /// </summary>
        private _VSFILECHANGEFLAGS fileChangeFlag;
        #endregion

        /// <summary>
        /// Constructs a new event args.
        /// </summary>
        /// <param name="fileName">File name that was changed on disk.</param>
        /// <param name="id">The item id of the file that was changed on disk.</param>
        internal FileChangedOnDiskEventArgs(string fileName, uint id, _VSFILECHANGEFLAGS flag) {
            this.fileName = fileName;
            this.itemID = id;
            this.fileChangeFlag = flag;
        }

        /// <summary>
        /// Gets the file name that was changed on disk.
        /// </summary>
        /// <value>The file that was changed on disk.</value>
        public string FileName {
            get {
                return this.fileName;
            }
        }

        /// <summary>
        /// Gets item id of the file that has changed
        /// </summary>
        /// <value>The file that was changed on disk.</value>
        internal uint ItemID {
            get {
                return this.itemID;
            }
        }

        /// <summary>
        /// The reason while the file has chnaged on disk.
        /// </summary>
        /// <value>The reason while the file has chnaged on disk.</value>
        public _VSFILECHANGEFLAGS FileChangeFlag {
            get {
                return this.fileChangeFlag;
            }
        }
    }
}
