import React from "react";
import Carousel from "react-bootstrap/Carousel";
import PropTypes from "prop-types";
import carouselBackground from "../../assets/images/carouselBack.JPG";
import "./providerDetails.css";

const formatPhoneNumber = phoneNumberString => {
  const cleaned = ("" + phoneNumberString).replace(/\D/g, "");
  const match = cleaned.match(/^(\d{3})(\d{3})(\d{4})$/);
  if (match) {
    return [match[1], "-", match[2], "-", match[3]].join("");
  }
  return null;
};

const ProviderPracticeCard = ({ practice }) => {
  return (
    <div>
      <img
        className="w-100"
        src={carouselBackground}
        alt="First slide"
        style={{ height: "492px", visibility: "hidden" }}
      />
      <Carousel.Caption>
        <h2 style={{ color: "black" }}>Practice Info</h2>
        <div className="row">
          <div className="border-right col-xs-12 col-sm-6 col-md-4 removeOverflow">
            <small className="text-muted">Name</small>
            <h6 style={{ color: "black" }}>{practice.name}</h6>
          </div>
          <div className="border-right col-xs-12 col-sm-6 col-md-4 removeOverflow">
            <small className="text-muted">Phone</small>
            <h6 style={{ color: "black" }}>
              {formatPhoneNumber(practice.phone)}
            </h6>
          </div>
          <div className="border-right col-xs-12 col-sm-6 col-md-4 removeOverflow">
            <small className="text-muted">Fax</small>
            <h6 style={{ color: "black" }}>
              {formatPhoneNumber(practice.fax)}
            </h6>
          </div>
        </div>
        <div className="row">
          <div className="col-xs-12 col-sm-6 col-md-4 removeOverflow border-right">
            <small className="text-muted">Facility Type</small>
            <h6 style={{ color: "black" }}>
              {practice.facilityType && practice.facilityType.name}
            </h6>
          </div>
          <div className="col-xs-12 col-sm-6 col-md-4 removeOverflow border-right">
            <small className="text-muted">Gender Accepted</small>
            <h6 style={{ color: "black" }}>
              {practice.genderAccepted && practice.genderAccepted.name}
            </h6>
          </div>
          <div className="col-xs-12 col-sm-6 col-md-4 removeOverflow">
            <small className="text-muted">ADA Accessible</small>
            <h6 style={{ color: "black" }}>
              {practice.isAdaAccessible && practice.isAdaAccessible
                ? "Yes"
                : "No"}
            </h6>
          </div>
        </div>
        <div className="row">
          <div className="col-xs-12 col-md-6 removeOverflow border-right">
            <small className="text-muted">Website</small>
            <h6 style={{ color: "black" }}>{practice.siteUrl}</h6>
          </div>
          <div className="col-xs-12 col-md-6 removeOverflow">
            <small className="text-muted">Email</small>
            <h6 style={{ color: "black" }}>{practice.email}</h6>
          </div>
        </div>
      </Carousel.Caption>
    </div>
  );
};

ProviderPracticeCard.propTypes = {
  practice: PropTypes.shape({
    name: PropTypes.string,
    phone: PropTypes.string,
    fax: PropTypes.string,
    email: PropTypes.string,
    siteUrl: PropTypes.string,
    locations: PropTypes.arrayOf(PropTypes.object),
    facilityType: PropTypes.shape({
      name: PropTypes.string
    }),
    genderAccepted: PropTypes.shape({
      name: PropTypes.string
    }),
    isAdaAccessible: PropTypes.bool
  })
};

export default ProviderPracticeCard;
