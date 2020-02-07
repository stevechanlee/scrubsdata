import React, { Component, Suspense } from "react";
import indexRoutes from "../routes/index";
import HigherLevelLoader from "./Loading/HigherLevelLoader";
import HigherLevelErrorBoundary from "./Loading/HigherLevelErrorBoundary";
import { Route, Switch, withRouter } from "react-router-dom";
import PropTypes from "prop-types";
import * as authService from "../services/authService";
import * as statusService from "../services/appStatusService";
import logger from "sabio-debug";
import Maintenance from "../views/authentication/Maintanance";

const _logger = logger.extend("AppRoutes");

class AppRoutes extends Component {
  constructor(props) {
    super(props);
    this.state = {
      currentUser: {
        roles: [],
        name: "",
        id: null,
        tenantId: null,
        isLoggedIn: false
      },
      inMaintenance: null
    };
    _logger("ctor");
  }

  static getDerivedStateFromProps(nextProps, prevState) {
    _logger("deriving state from props");

    const { state } = nextProps.location;
    if (state) {
      if (
        state.type === "LOGOUT" &&
        prevState.currentUser.isLoggedIn !== false
      ) {
        return { currentUser: { ...state.currentUser, roles: [] } };
      } else if (
        state.type === "LOGIN" &&
        prevState.currentUser.isLoggedIn !== true
      ) {
        return { currentUser: state.currentUser };
      }
    }

    return null;
  }

  componentDidMount() {
    _logger("mounting");
    authService
      .getCurrentUser()
      .then(this.onUserConfirmed)
      .catch(this.onUserNotConfirmed);

    statusService
      .get(1)
      .then(this.getStatusSuccess)
      .catch(this.getStatusError);
  }

  onUserConfirmed = res => {
    _logger("this is the approutes user response", res);
    this.setState({ currentUser: { ...res.item, isLoggedIn: true } });
  };

  onUserNotConfirmed = err => {
    _logger("no session", err);
  };

  getStatusSuccess = ({ item }) => {
    this.setState({
      inMaintenance: item.statusId
    });
  };

  getStatusError = errRes => {
    _logger(errRes);
  };

  render() {
    return (
      <HigherLevelErrorBoundary>
        <Suspense fallback={<HigherLevelLoader />}>
          <Switch>
            {this.state.inMaintenance === 3 &&
            !this.state.currentUser.roles.includes("SysAdmin") ? (
              <Route to="/maintenance" component={Maintenance} />
            ) : (
              indexRoutes
                .filter(route => {
                  return (
                    route.authorized ===
                    (this.state.currentUser.id ? true : false)
                  );
                })
                .map((prop, key) => {
                  var Component = prop.component;
                  return (
                    <Route
                      key={key}
                      path={prop.path}
                      render={props => {
                        return (
                          <Component
                            {...this.props}
                            {...props}
                            currentUser={this.state.currentUser}
                            roles={prop.roles ? prop.roles : ""}
                          />
                        );
                      }}
                    />
                  );
                })
            )}
          </Switch>
        </Suspense>
      </HigherLevelErrorBoundary>
    );
  }
}

export default withRouter(AppRoutes);

AppRoutes.propTypes = {
  history: PropTypes.shape({ push: PropTypes.func, replace: PropTypes.func }),
  location: PropTypes.shape({
    state: PropTypes.shape({
      type: PropTypes.string,
      currentUser: PropTypes.shape({})
    })
  })
};
