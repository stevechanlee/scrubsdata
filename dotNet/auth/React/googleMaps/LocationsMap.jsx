import { withGoogleMap, GoogleMap, Marker } from "react-google-maps";
import React from "react";
import PropTypes from "prop-types";

export const LocationsMap = props => {
  return (
    <GoogleMap
      defaultZoom={props.zoom}
      defaultCenter={{
        lat: props.latitude,
        lng: props.longitude
      }}
    >
      {props.coords.map((location, index) => {
        return (
          <Marker
            key={index}
            position={{
              lat: location.latitude,
              lng: location.longitude
            }}
          />
        );
      })}
    </GoogleMap>
  );
};

export const Map = withGoogleMap(LocationsMap);

LocationsMap.propTypes = {
  latitude: PropTypes.number,
  longitude: PropTypes.number,
  zoom: PropTypes.number,
  coords: PropTypes.arrayOf(
    PropTypes.shape({
      latitude: PropTypes.number,
      longitude: PropTypes.number
    })
  )
};
