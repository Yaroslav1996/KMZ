var map;
var ctaLayer;
function initialize()
{
    map = new google.maps.Map(document.getElementById('map'),
        {
            zoom: 12,
            center: new google.maps.LatLng(49.9040968423402, 20.2699492565228),
            mapTypeId: google.maps.MapTypeId.ROADMAP,
            disableDefaultUI: true,
            zoomControl: true,
            maxZoom: 19
        });
}