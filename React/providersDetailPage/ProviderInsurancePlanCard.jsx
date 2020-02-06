import React from "react";
import PropTypes from "prop-types";
// import logger from "sabio-debug";

// const _logger = logger.extend("ProviderInsurancePlanCard");

const ProviderInsurancePlanCard = ({ insurancePlan }) => {
  return (
    <React.Fragment>
      <div className="row">
        <div className="col-6 col-md-3">
          <strong>Name</strong>
          <br />
          <p className="text-muted">{insurancePlan && insurancePlan.name}</p>
        </div>
        <div className="col-6 col-md-3">
          <strong>Code</strong>
          <br />
          <p className="text-muted">{insurancePlan && insurancePlan.code}</p>
        </div>
        <div className="col-6 col-md-3">
          <strong>Min Age</strong>
          <br />
          <p className="text-muted">{insurancePlan && insurancePlan.minAge}</p>
        </div>
        <div className="col-6 col-md-3">
          <strong>Max Age</strong>
          <br />
          <p className="text-muted">{insurancePlan && insurancePlan.maxAge}</p>
        </div>
      </div>
      <div className="row">
        <div className="col">
          <strong>Plan Level</strong>
          <p className="text-muted">
            {insurancePlan.planLevel && insurancePlan.planLevel.name}
          </p>
          <strong>Plan Type</strong>
          <p className="text-muted">
            {insurancePlan.planType && insurancePlan.planType.name}
          </p>
          <strong>PlanStatus</strong>
          <p className="text-muted">
            {insurancePlan.planStatus && insurancePlan.planStatus.name}
          </p>
        </div>
      </div>
    </React.Fragment>
  );
};

ProviderInsurancePlanCard.propTypes = {
  insurancePlan: PropTypes.shape({
    name: PropTypes.string,
    code: PropTypes.string,
    minAge: PropTypes.number,
    maxAge: PropTypes.number,
    planLevel: PropTypes.shape({
      name: PropTypes.string
    }),
    planStatus: PropTypes.shape({
      name: PropTypes.string
    }),
    planType: PropTypes.shape({
      name: PropTypes.string
    })
  })
};

export default ProviderInsurancePlanCard;
