import React from "react";
import PropTypes from "prop-types";
import { Map } from "../locations/LocationsMap";

const ProviderPracticeLocationCard = ({ location }) => {
  const locationMapper = (location, i) => {
    return (
      <div className="col-md-4 col-xs-12" key={i}>
        <small className="text-muted pt-3 db">Address</small>
        <h6>
          {location.state &&
            `${location.lineOne} ${location.lineTwo}, ${location.state.code} ${location.zip}`}
        </h6>
        <div>
          <Map
            loadingElement={<div style={{ height: `100%` }} />}
            containerElement={<div style={{ height: `150px` }} />}
            mapElement={<div style={{ height: `100%` }} />}
            latitude={location.latitude}
            longitude={location.longitude}
            zoom={12}
            coords={[
              {
                latitude: location.latitude,
                longitude: location.longitude
              }
            ]}
            key={`map_${i}`}
          />
        </div>
      </div>
    );
  };

  return <>{location && location.map(locationMapper)}</>;
};

export default ProviderPracticeLocationCard;

ProviderPracticeLocationCard.propTypes = {
  location: PropTypes.shape({
    schedule: PropTypes.arrayOf(
      PropTypes.shape({
        map: PropTypes.func,
        dayOfWeek: PropTypes.number,
        time: PropTypes.arrayOf(
          PropTypes.shape({
            id: PropTypes.number,
            startTime: PropTypes.string,
            endTime: PropTypes.string
          })
        )
      })
    ),
    map: PropTypes.func,
    id: PropTypes.number,
    city: PropTypes.string,
    lineOne: PropTypes.string,
    lineTwo: PropTypes.string,
    zip: PropTypes.string,
    latitude: PropTypes.number,
    longitude: PropTypes.number,
    state: PropTypes.shape({
      id: PropTypes.number,
      name: PropTypes.string,
      code: PropTypes.string
    })
  })
};
