import React from "react";
import debug from "sabio-debug";
import * as providersService from "../../services/providersService";
import PropType from "prop-types";
import { TabContent, TabPane, Nav, NavItem, NavLink } from "reactstrap";
import classnames from "classnames";
import ProviderPracticeCard from "./ProviderPracticeCard";
import ProviderInsurancePlanCard from "./ProviderInsurancePlanCard";
import Carousel from "react-bootstrap/Carousel";
import "./providerDetails.css";
import ProviderPracticeLocationCard from "./ProviderPracticeLocationCard";
import ScheduleAvailabilityCard from "../scheduleAvailability/ScheduleAvailabilityCard";
import * as authService from "../../services/authService";
import * as providerService from "../../services/providersService";
import Swal from "sweetalert2";
const logger = debug.extend("ProviderDetailsPage");

class ProviderDetailsPage extends React.Component {
  state = {
    profile: {
      userProfile: {
        avatarUrl: "",
        firstName: "",
        lastName: "",
        mi: ""
      },
      id: 0,
      phone: "",
      fax: "",
      title: {},
      gender: {},
      professionalDetails: {},
      practices: [],
      insurancePlans: []
    },
    providerId: 0,
    activeTab: "1"
  };

  componentDidMount() {
    logger("==================");

    if (this.props.match.params.id) {
      const { params } = this.props.match;
      this.setState(prevState => {
        return {
          ...prevState,
          providerId: params.id
        };
      });
      this.getProviderDetails(params.id);
    } else {
      authService
        .getProviderId()
        .then(this.getProviderSuccess)
        .catch(this.getProviderFail);
    }
  }

  getProviderSuccess = ({ item }) => {
    this.setState(prevState => {
      return {
        ...prevState,
        providerId: item
      };
    });
    this.getProviderDetails(item);
  };

  getProviderFail = () => {};

  getProviderDetails = id => {
    providersService
      .getProviderDetails(id)
      .then(this.getProviderDetailsSuccess)
      .catch(this.getProviderDetailsError);
  };

  getProviderDetailsSuccess = res => {
    logger(res);
    this.setState(() => {
      return {
        profile: res.item
      };
    });
  };

  getProviderDetailsError = () => {
    logger("error on get provider details");
  };

  toggle = tab => {
    if (this.state.activeTab !== tab) {
      this.setState(() => {
        return {
          activeTab: tab
        };
      });
    }
  };

  practiceCardMapper = (practice, i) => {
    return (
      <Carousel.Item
        key={`carouselItem_${i}`}
        style={{ marginBottom: "30px", height: "492px" }}
      >
        <ProviderPracticeCard key={`prac-info-${i}`} practice={practice} />
      </Carousel.Item>
    );
  };

  practiceLocationMapper = (practice, i) => {
    return (
      <ProviderPracticeLocationCard
        key={`prov-prac-${i}`}
        location={practice.locations}
      />
    );
  };

  ScheduleAvailabilityMapper = (location, i) => {
    return (
      <ScheduleAvailabilityCard key={`prac-schd-${i}`} location={location} />
    );
  };

  insurancePlanMapper = (insurancePlan, i) => {
    return (
      <ProviderInsurancePlanCard
        key={`prov-insuranceplan-${i}`}
        insurancePlan={insurancePlan}
      />
    );
  };

  renderProfessionalDetails = () => {
    const { professionalDetails } = this.state.profile;
    let genders;
    if (!professionalDetails) {
      return;
    }
    if (professionalDetails.genderAccepted) {
      if (professionalDetails.genderAccepted.name) {
        genders = professionalDetails.genderAccepted.name;
      }
    }

    return (
      <div className="row">
        <div className="col-6">
          <small className="text-muted pt-4 db">Gender Accepted</small>
          {genders && <h6>{genders}</h6>}
        </div>
        <div className="col-6">
          <small className="text-muted pt-4 db">NPI</small>
          <h6>{professionalDetails.npi && professionalDetails.npi}</h6>
        </div>
      </div>
    );
  };
  updateDateAttested = () => {
    providerService
      .updateLastAttested(parseInt(this.props.match.params.id))
      .then(this.updateDateSuccess)
      .catch(this.updateDateFail);
  };
  updateDateSuccess = id => {
    logger("success", id);
    Swal.fire("Successful!", "This Provider is compliant", "success");
  };
  updateDateFail = resp => {
    logger("fail", resp);
  };
  render() {
    const { profile, activeTab } = this.state;
    return (
      <React.Fragment>
        <div className="row m-1 mb-3" style={{ backgroundColor: "white" }}>
          <div className="col-12 col-lg-4 removeOverflow">
            <div className="row">
              <div className="col-12 p-3">
                <img
                  src={
                    profile.userProfile
                      ? profile.userProfile.avatarUrl
                      : "https://icon-library.net/images/no-profile-picture-icon/no-profile-picture-icon-13.jpg"
                  }
                  className="rounded-circle frame"
                  width={150}
                  height={150}
                  alt={
                    profile.userProfile &&
                    `${profile.userProfile.firstName} ${profile.userProfile.lastName}`
                  }
                />
                <div className="mt-2 card-title">
                  {profile.userProfile &&
                    `${profile.userProfile.firstName} ${profile.userProfile.lastName}`}
                </div>
                <div className="card-subtitle">
                  {profile.title && profile.title.name}
                </div>
              </div>
            </div>
            <div className="row border-top">
              <div className="col-4">
                <small className="text-muted">Gender</small>
                <h6>{profile.gender.name && profile.gender.name}</h6>
              </div>
              <div className="col-4">
                <small className="text-muted pt-4 db">Phone</small>
                <h6>{profile.phone && profile.phone}</h6>
              </div>
              <div className="col-4">
                <small className="text-muted pt-4 db">Fax</small>
                <h6>{profile.fax && profile.fax}</h6>
              </div>
            </div>

            <div className="row">
              <div className="col-4">
                <small className="text-muted pt-4 db">Specialization</small>
                {profile.specializations &&
                  profile.specializations.map((ele, i) => (
                    <h6 key={`specializations_${i}`}>{ele.name}</h6>
                  ))}
              </div>
              <div className="col-4">
                <small className="text-muted pt-4 db">Language</small>
                {profile.languages &&
                  profile.languages.map((ele, i) => (
                    <h6 key={`language_${i}`}>{ele.name}</h6>
                  ))}
              </div>
            </div>

            {this.renderProfessionalDetails()}
          </div>

          <div className="col-12 col-lg-8 border">
            <Carousel className="carousel">
              {profile.practices &&
                profile.practices.map(this.practiceCardMapper)}
            </Carousel>
            <button
              type="button"
              round="true"
              icon="true"
              className="btn btn-inverse btn-sm"
              onClick={() =>
                this.props.history.push("/practices/new", this.state.providerId)
              }
            >
              <i className="fa fa-plus"></i>
            </button>
          </div>
        </div>

        <div className="row">
          <div className="col-12">
            <div className="card p-3 border">
              <Nav tabs>
                <NavItem>
                  <NavLink
                    className={classnames({
                      active: activeTab === "1"
                    })}
                    onClick={() => {
                      this.toggle("1");
                    }}
                  >
                    Locations
                  </NavLink>
                </NavItem>
                <NavItem>
                  <NavLink
                    className={classnames({
                      active: activeTab === "2"
                    })}
                    onClick={() => {
                      this.toggle("2");
                    }}
                  >
                    Insurance Plans
                  </NavLink>
                </NavItem>
                <NavItem>
                  <NavLink
                    className={classnames({
                      active: activeTab === "3"
                    })}
                    onClick={() => {
                      this.toggle("3");
                    }}
                  >
                    Affiliations
                  </NavLink>
                </NavItem>
                <NavItem>
                  <NavLink
                    className={classnames({
                      active: activeTab === "4"
                    })}
                    onClick={() => {
                      this.toggle("4");
                    }}
                  >
                    Certifications/Licenses
                  </NavLink>
                </NavItem>
                <NavItem>
                  <NavLink
                    className={classnames({
                      active: this.state.activeTab === "5"
                    })}
                    onClick={() => {
                      this.toggle("5");
                    }}
                  >
                    Schedules
                  </NavLink>
                </NavItem>
                <NavItem>
                  <NavLink
                    className={classnames({
                      active: this.state.activeTab === "6"
                    })}
                    onClick={() => {
                      this.toggle("6");
                    }}
                  >
                    Attest
                  </NavLink>
                </NavItem>
              </Nav>
              <TabContent activeTab={this.state.activeTab} className="pt-3">
                <TabPane tabId="1" className="text-center">
                  <h3>
                    <u>Locations</u>
                  </h3>
                  <div className="row">
                    {profile.practices &&
                      profile.practices.map(this.practiceLocationMapper)}
                  </div>
                </TabPane>
                <TabPane tabId="2" className="text-center">
                  <div className="row p-3">
                    <div className="col-sm-12">
                      <h3>
                        <u>Insurance Plans</u>
                      </h3>
                      {profile.insurancePlans &&
                        profile.insurancePlans.map(this.insurancePlanMapper)}
                    </div>
                  </div>
                </TabPane>
                <TabPane tabId="3" className="text-center">
                  <div className="row p-3">
                    <div className="col-sm-12">
                      <h3>
                        <u>Affiliations</u>
                      </h3>
                      {profile.affiliations &&
                        profile.affiliations.map((ele, i) => (
                          <React.Fragment key={`affiliations_${i}`}>
                            <h4 className="mt-4">{ele.name}</h4>
                            <p className="text-muted">
                              {ele.affiliationType.name}
                            </p>
                          </React.Fragment>
                        ))}
                    </div>
                  </div>
                </TabPane>
                <TabPane tabId="4" className="text-center">
                  <div className="row p-3">
                    <div className="col-sm-12">
                      <h3>
                        <u>Certifications</u>
                      </h3>
                      {profile.certifications &&
                        profile.certifications.map((ele, i) => (
                          <p key={`certifications_${i}`}>{ele.name}</p>
                        ))}
                      <h3 className="pt-3">
                        <u>Licenses</u>{" "}
                        <button
                          type="button"
                          round="true"
                          icon="true"
                          className="btn btn-inverse btn-sm"
                          onClick={() =>
                            this.props.history.push(
                              "/licenses/new",
                              this.state.providerId
                            )
                          }
                        >
                          <i className="fa fa-plus"></i>
                        </button>
                      </h3>
                      {profile.licenses &&
                        profile.licenses.map((ele, i) => (
                          <p key={`licenses_${i}`}>
                            {ele.state.name} | {ele.licenseNumber} |{" "}
                            <button
                              type="button"
                              id={ele.id}
                              round="true"
                              icon="true"
                              className="btn btn-inverse btn-sm"
                              onClick={() =>
                                this.props.history.push(
                                  `/licenses/${ele.id}/edit`,
                                  ele
                                )
                              }
                            >
                              <i className="fa fa-edit"></i>
                            </button>
                          </p>
                        ))}
                    </div>
                  </div>
                </TabPane>
                <TabPane tabId="5" className="text-center">
                  <h3>
                    <u>Schedules</u>{" "}
                    <button
                      type="button"
                      round="true"
                      icon="true"
                      className="btn btn-inverse btn-sm"
                      onClick={() =>
                        this.props.history.push(
                          "/scheduleForm",
                          this.state.providerId
                        )
                      }
                    >
                      <i className="fa fa-plus"></i>
                    </button>
                  </h3>

                  <div className="row">
                    {profile.practices &&
                      profile.practices.map(this.ScheduleAvailabilityMapper)}
                  </div>
                </TabPane>
                <TabPane tabId="6" className="text-center">
                  <p>
                    I attest that all my information is correct, up to date, and
                    has been updated within the last 90 days.{" "}
                    <button
                      type="button"
                      round="true"
                      icon="true"
                      className="btn btn-inverse btn-sm"
                      onClick={this.updateDateAttested}
                    >
                      <i className="fa fa-check"></i>
                    </button>
                  </p>
                </TabPane>
              </TabContent>
            </div>
          </div>
        </div>
      </React.Fragment>
    );
  }
}

ProviderDetailsPage.propTypes = {
  match: PropType.shape({
    params: PropType.shape({
      id: PropType.string
    })
  }),
  currentUser: PropType.shape({
    id: PropType.number,
    roles: PropType.arrayOf(PropType.string)
  }),
  history: PropType.shape({
    push: PropType.func
  })
};

export default ProviderDetailsPage;
