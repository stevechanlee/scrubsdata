import React from "react";
import PropTypes from "prop-types";
import "./InsurancePlan.css";

const InsurancePlanCard = props => {
  const handleEdit = () => {
    props.handleEdit(props.insurancePlans);
  };

  const handleDelete = () => {
    props.handleDelete(props.insurancePlans.id);
  };

  const renderButtons = () => {
    return props.currentUserRoles.includes("SysAdmin") ? (
      <React.Fragment>
        <td>
          <span>
            <button
              type="button"
              round="true"
              icon="true"
              className="btn btn-inverse btn-sm text-white"
              onClick={handleEdit}
            >
              <i className="fa fa-edit"></i>
            </button>
          </span>
        </td>
        <td>
          <span>
            <button
              type="button"
              round="true"
              icon="true"
              className="btn btn-inverse btn-sm text-white"
              onClick={handleDelete}
            >
              <i className="fas fa-trash"></i>
            </button>
          </span>
        </td>
      </React.Fragment>
    ) : null;
  };

  return (
    <React.Fragment>
      <tr>
        <td>
          <span>{props.insurancePlans && props.insurancePlans.name}</span>
        </td>
        <td>
          <span> {props.insurancePlans && props.insurancePlans.code}</span>
        </td>
        <td>
          <span>
            {" "}
            {props.insurancePlans && props.insurancePlans.planLevel.name}
          </span>
        </td>
        <td>
          <span>
            {props.insurancePlans && props.insurancePlans.planType.name}
          </span>
        </td>
        <td>
          <span>
            {props.insurancePlans && props.insurancePlans.planType.code}
          </span>
        </td>
        <td>
          <span>
            {props.insurancePlans &&
              props.insurancePlans.insuranceProvider.name}
          </span>
        </td>
        <td>
          <span>
            {props.insurancePlans &&
              props.insurancePlans.insuranceProvider.siteUrl}
          </span>
        </td>
        <td>
          <span>
            {props.insurancePlans && props.insurancePlans.planStatus.name}
          </span>
        </td>
        {renderButtons()}
      </tr>
    </React.Fragment>
  );
};

InsurancePlanCard.propTypes = {
  insurancePlans: PropTypes.shape({
    id: PropTypes.number,
    name: PropTypes.string,
    code: PropTypes.string,
    minAge: PropTypes.number,
    maxAge: PropTypes.number,
    insuranceProvider: PropTypes.shape({
      name: PropTypes.string,
      siteUrl: PropTypes.string
    }),
    planLevel: PropTypes.shape({
      name: PropTypes.string
    }),
    planType: PropTypes.shape({
      name: PropTypes.string,
      code: PropTypes.string
    }),
    planStatus: PropTypes.shape({
      name: PropTypes.string
    })
  }),
  handleEdit: PropTypes.func,
  handleDelete: PropTypes.func,
  currentUserRoles: PropTypes.arrayOf(PropTypes.string)
};

export default InsurancePlanCard;
