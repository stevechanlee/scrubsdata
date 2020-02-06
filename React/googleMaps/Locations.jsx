import logger from "sabio-debug";
import React from "react";
import LocationCard from "./LocationCard";
import * as locationServices from "../../services/locationServices";
import { getByUserId } from "../../services/scheduleService";
import Pagination from "rc-pagination";
import localeInfo from "rc-pagination/lib/locale/en_US";
import "rc-pagination/assets/index.css";
import PropTypes from "prop-types";
import Swal from "sweetalert2";
import withReactContent from "sweetalert2-react-content";
import { Map } from "./LocationsMap";

const _logger = logger.extend("Locations");

class Locations extends React.Component {
  state = {
    locationArray: [],
    locationTemplate: [],
    currentPage: 1,
    resultsPerPage: 20,
    total: 0,
    location: {},
    locationTypes: [],
    states: [],
    viewForm: false,
    mappedCoords: [],
    providerId: 0
  };

  componentDidMount() {
    getByUserId()
      .then(this.onGetSuccess)
      .catch(this.onError);
  }

  onGetSuccess = res => {
    this.setState(
      prevState => {
        return { ...prevState, providerId: res.items[0].providerId };
      },
      () => this.setupList()
    );
  };

  onError = err => {
    _logger("We had an error", err);
    Swal.fire(
      "Error",
      "It looks like you don't have any locations tied to your account.",
      "error"
    );
  };

  displayAllLocations = page => {
    locationServices
      .getAllByProviderId(
        page,
        this.state.resultsPerPage,
        this.state.providerId
      )
      .then(this.onDisplayLocationSuccess)
      .catch(this.onGenericError);
  };

  onDisplayLocationSuccess = res => {
    let rawLocations = res.data.item.pagedItems;
    this.setState(prevState => {
      return {
        ...prevState,
        locationArray: rawLocations,
        locationTemplate: rawLocations.map(this.renderLocation),
        total: res.data.item.totalCount,
        mappedCoords: rawLocations.map(this.renderCoordinates)
      };
    });
  };

  renderLocation = location => (
    <LocationCard
      key={"Locations_" + location.id}
      locationData={location}
      editLocation={this.editLocationButton}
      deleteLocation={this.onDeletePrompt}
      viewLocation={this.onViewMore}
    />
  );

  renderCoordinates = ({ latitude, longitude }) => {
    return { latitude, longitude };
  };

  setupList = () => {
    this.displayAllLocations(this.state.currentPage - 1);
  };

  onPageChange = page => {
    this.setState({ currentPage: page }, this.setupList);
  };

  editLocationButton = locationData => {
    this.props.history.push(`/locations/${locationData.id}/edit`, locationData);
  };

  addLocationButton = () => {
    this.props.history.push(`/locations/new`);
  };

  onDeletePrompt = locationId => {
    const mySwal = withReactContent(Swal);
    mySwal
      .fire({
        title: "Are you sure?",
        text: "This location will be deleted",
        type: "warning",
        showCancelButton: true,
        confirmButtonColor: "#DD6B55",
        confirmButtonText: "Yes, delete it!",
        cancelButtonText: "No, cancel please!",
        closeOnConfirm: false,
        closeOnCancel: false
      })
      .then(result => {
        if (result.value) {
          this.deleteLocationConfirm(locationId);
        } else if (result.dismiss === Swal.DismissReason.cancel) {
          Swal.fire("Cancelled", "No Location Removed.", "error");
        }
      });
  };

  deleteLocationConfirm = locationId => {
    locationServices
      .deleteById(locationId)
      .then(this.onDeleteSuccess)
      .catch(this.onGenericError);
  };

  onDeleteSuccess = res => {
    this.setState(prevState => {
      const locationArray = prevState.locationArray.filter(
        item => item.id !== res
      );
      return {
        ...prevState,
        locationArray,
        locationTemplate: locationArray.map(this.renderLocation)
      };
    });
  };

  onGenericError = err => {
    Swal.fire(
      "Error",
      "This location is currently tied to a schedule. Please remove this location from all schedules prior to deleting it.",
      "error"
    );
    _logger("We had an error with your request", err);
  };

  onViewMore = locationData => {
    this.props.history.push(`/locations/${locationData.id}/view`, locationData);
  };

  render() {
    return (
      <React.Fragment>
        <div>
          <div className="card-body">
            <div className="row d-flex justify-content-center">
              <h5 className="display-7">Locations Near You</h5>
            </div>

            <Map
              loadingElement={<div style={{ height: `100%` }} />}
              containerElement={<div style={{ height: `400px` }} />}
              mapElement={<div style={{ height: `100%` }} />}
              latitude={33.684566}
              longitude={-117.826508}
              zoom={12}
              coords={
                this.state.mappedCoords
                  ? this.state.mappedCoords
                  : { latitude: 33.684566, longitude: -117.826508 }
              }
            />

            <br />
          </div>

          <div className="row d-flex justify-content-center">
            <br />
            {this.state.locationTemplate}
          </div>
          <div className="row d-flex justify-content-center">
            <Pagination
              onChange={this.onPageChange}
              current={this.state.currentPage}
              total={this.state.total}
              defaultPageSize={this.state.resultsPerPage}
              locale={localeInfo}
            />
          </div>
        </div>
      </React.Fragment>
    );
  }
}

Locations.propTypes = {
  history: PropTypes.shape({
    push: PropTypes.func
  }),
  match: PropTypes.shape({
    params: PropTypes.shape({
      id: PropTypes.string
    })
  }),

  locationData: PropTypes.shape({
    id: PropTypes.number,
    city: PropTypes.string,
    lineOne: PropTypes.string,
    lineTwo: PropTypes.string,
    state: PropTypes.shape({ name: PropTypes.string }),
    zip: PropTypes.string,
    latitude: PropTypes.number,
    longitude: PropTypes.number
  })
};

export default Locations;
