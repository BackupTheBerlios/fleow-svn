
using System;
using Mono.Unix;
 
using Banshee.Base;
using Banshee.Sources;
 
namespace Banshee.Plugins.Fleow
{
    public class FleowPlugin : Banshee.Plugins.Plugin
    {
        protected override string ConfigurationName { get { return "Fleow"; } }
        public override string DisplayName { get { return "Fleow"; } }
        
        public override string Description {
            get {
                return Catalog.GetString(
                    "A Banshee plugin letting you to view, manage &" +
                    " select albums from the library in the 3D mode."
                );
            }
        }
        
        public override string [] Authors {
            get {
                return new string [] {
                    "Łukasz Wiśniewski <vishna@gazeta.pl>"
                };
            }
        }
 
        // --------------------------------------------------------------- //
        
        private FleowPane fleow_pane;
        private uint timeout_id;
        private SampleSource source;
        
        protected override void PluginInitialize()
        {
            Console.WriteLine("Initializing Sample Plugin");
            timeout_id = GLib.Timeout.Add(5000, OnTimeout);
            source = new SampleSource();
            SourceManager.AddSource(source);
        }
        
        // optional, this is a virtual override, only
        // only provide an implementation if there are
        // resources to dispose of or other objects that
        // need notification, etc.
        protected override void PluginDispose()
        {
            Console.WriteLine("Disposing Sample Plugin");
            GLib.Source.Remove(timeout_id);
            timeout_id = 0;
            SourceManager.RemoveSource(source);
				InstallInterfaceElements();
        }
        
        // optional, this is a virtual override, only 
        // provide an implementation if there is a 
        // configuration GUI to show
        private Gtk.Button button = new Gtk.Button("Configure Me");
        
        public override Gtk.Widget GetConfigurationWidget()
        {
            return button;    
        }
        
        private bool OnTimeout()
        {
            int track_id = Convert.ToInt32(Globals.Library.Db.QuerySingle(
                "SELECT TrackID FROM Tracks ORDER BY RANDOM() LIMIT 1"));
            Console.WriteLine(Globals.Library.GetTrack(track_id));
            return true;
        }

       // --------------------------------------------------------------- //
        
        private void InstallInterfaceElements()
        {
            fleow_pane = new FleowPane();
            InterfaceElements.MainContainer.PackEnd(fleow_pane, false, false, 0);
        }
    }
    
    public class SampleSource : Banshee.Sources.Source
    {
        private Gtk.Button show_tracks = new Gtk.Button("Change View");
    
        public SampleSource() : base("Sample Source", 150)
        {
            show_tracks.Clicked += delegate(object o, EventArgs args) {
                show_tracks = null;
                OnViewChanged();
            };
        }
        
        public override Gtk.Widget ViewWidget {
            get {
                return show_tracks;
            }
        }
    }
}
