using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using MapTileProject.CustomControl;
using MapTileProject.Droid.CustomRenderer;
using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Maps.Android;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CustomMap), typeof(CustomMapRenderer))]
namespace MapTileProject.Droid.CustomRenderer
{
    /// <summary>
    /// CustomRenderer for the CustomMap created in the PCL part.
    /// This Renderer gives us the possibility to add/override some functionalities.
    /// </summary>
    public class CustomMapRenderer : MapRenderer, IOnMapReadyCallback
    {
        /// <summary>
        /// Instance of our Custom control declared in the PCL part.
        /// </summary>
        CustomMap customMap;
        /// <summary>
        /// Instance of the native map for this plateform.
        /// </summary>
        GoogleMap nativeMap;

        /// <summary>
        /// We override the OnElementChanged() event handler to get the desired instance. We also use it for updates.
        /// </summary>
        /// <param name="e">It contains either the NewElement or the OldElement</param>
        protected override void OnElementChanged(ElementChangedEventArgs<View> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                // Unsubscribe
            }

            if (e.NewElement != null)
            {
                customMap = e.NewElement as CustomMap;
                ((MapView)Control).GetMapAsync(this);
            }
        }

        /// <summary>
        /// The on element property changed callback.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/>Instance containing the event data.</param>
        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == CustomMap.MapTileTemplateProperty.PropertyName)
                UpdateTiles();
        }

        /// <summary>
        /// Tile overlay instance
        /// </summary>
        private TileOverlay tileOverlay;
        /// <summary>
        /// This function update the tiles of the Map for this plateform.
        /// </summary>
        private void UpdateTiles()
        {
            if (nativeMap != null)
            {
                if (this.customMap.MapTileTemplate != null)
                {
                    if (this.tileOverlay != null)
                    {
                        this.tileOverlay.Remove();
                        this.nativeMap.MapType = GoogleMap.MapTypeNormal;
                    }

                    this.nativeMap.MapType = GoogleMap.MapTypeNone;

                    this.tileOverlay = this.nativeMap.AddTileOverlay(
                        new TileOverlayOptions()
                            .InvokeTileProvider(
                                new CustomTileProvider(512, 512, customMap.MapTileTemplate))
                            .InvokeZIndex(-1));
                }
            }
        }

        /// <summary>
        /// This function only takes place on Android plateform. This function is the native callback called when the map is loaded.
        /// </summary>
        /// <param name="googleMap"></param>
        public void OnMapReady(GoogleMap googleMap)
        {
            this.nativeMap = googleMap;
            googleMap.UiSettings.ZoomControlsEnabled = false;
            
            UpdateTiles();
        }

        /// <summary>
        /// This class converts the basic url template value (x, y, z) into real values.
        /// </summary>
        public class CustomTileProvider : UrlTileProvider
        {
            private string urlTemplate;

            public CustomTileProvider(int x, int y, string urlTemplate)
            : base(x, y)
            {
                this.urlTemplate = urlTemplate;
            }

            public override Java.Net.URL GetTileUrl(int x, int y, int z)
            {
                var url = urlTemplate.Replace("{z}", z.ToString()).Replace("{x}", x.ToString()).Replace("{y}", y.ToString());
                return new Java.Net.URL(url);
            }
        }
    }
}