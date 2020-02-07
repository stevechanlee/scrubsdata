import React from "react";
import { Form, FormGroup, Label, Button } from "reactstrap";
import { Formik, Field } from "formik";
import logger from "sabio-debug";
import propTypes from "prop-types";
import Swal from "sweetalert2";
import * as insurancePlansSchema from "../insurancePlans/insurancePlansSchema";
import * as insurancePlansService from "../../services/insurancePlansService";
import * as insurancePlansProviderService from "../../services/insuranceProviderService";

const _logger = logger.extend("InsurancePlansForm");

class InsurancePlansForm extends React.Component {
  state = {
    plansProvider: [],
    plansLevelType: [],
    plansType: [],
    plansStatus: [],
    isEditting: false,
    formData: {
      insuranceProviderId: 0,
      name: "",
      planTypeId: 0,
      levelTypeId: 0,
      code: "",
      planStatusId: 0,
      minAge: 0,
      maxAge: 0
    }
  };

  componentDidMount = () => {
    if (this.props.match.params.id) {
      this.getById(this.props.match.params.id);
      this.setState(() => {
        return {
          isEditting: true
        };
      });
    }
    this.getAllProviders();
    this.getAllPlansLevelType();
    this.getAllPlansType();
    this.getAllPlansStatus();
  };

  getAllProviders = () => {
    insurancePlansProviderService
      .getAll()
      .then(this.getAllProvidersSuccess)
      .catch(this.getAllProvidersError);
  };

  getAllProvidersSuccess = res => {
    this.setState(() => {
      return {
        plansProvider: res.items
      };
    });
  };

  getAllProvidersError = () => {
    _logger("error on get all providers");
  };

  getAllPlansLevelType = () => {
    insurancePlansService
      .getAllPlansLevelType()
      .then(this.getAllPlansLevelTypeSuccess)
      .catch(this.getAllPlansLevelTypeError);
  };

  getAllPlansLevelTypeSuccess = res => {
    this.setState(() => {
      return {
        plansLevelType: res.items.map(ele => ele)
      };
    });
  };

  getAllPlansLevelTypeError = () => {
    _logger("error on get all plans level");
  };

  getAllPlansType = () => {
    insurancePlansService
      .getAllPlansType()
      .then(this.getAllPlansTypeSuccess)
      .catch(this.getAllPlansTypeError);
  };

  getAllPlansTypeSuccess = res => {
    this.setState(() => {
      return {
        plansType: res.items.map(ele => ele)
      };
    });
  };

  getAllPlansTypeError = () => {
    _logger("error on get all plans type");
  };

  getAllPlansStatus = () => {
    insurancePlansService
      .getAllPlansStatus()
      .then(this.getAllPlansStatusSuccess)
      .catch(this.getAllPlansStatusError);
  };

  getAllPlansStatusSuccess = res => {
    this.setState(() => {
      return {
        plansStatus: res.items.map(ele => ele)
      };
    });
  };

  getAllPlansStatusTypeError = () => {
    _logger("error on get all plans status");
  };

  getById = id => {
    insurancePlansService
      .getById(id)
      .then(this.getByIdSuccess)
      .catch(this.getByIdError);
  };

  getByIdSuccess = res => {
    this.setState(() => {
      return {
        formData: {
          id: res.item.id,
          insuranceProviderId: res.item.insuranceProvider.id,
          name: res.item.name,
          planTypeId: res.item.planType.id,
          levelTypeId: res.item.planLevel.id,
          code: res.item.code,
          planStatusId: res.item.planStatus.id,
          minAge: res.item.minAge,
          maxAge: res.item.maxAge
        }
      };
    });
  };

  getByIdError = () => {
    _logger("error on get by id");
  };

  handleSubmit = (values, { resetForm }) => {
    if (this.state.isEditting) {
      insurancePlansService
        .update(values)
        .then(this.handleUpdateSuccess)
        .catch(this.handeUpdateError);
      resetForm(this.state.formData);
    } else {
      insurancePlansService
        .post(values)
        .then(this.handleSubmitSuccess)
        .catch(this.handleSubmitError);
      resetForm(this.state.formData);
    }
  };

  handleSubmitSuccess = () => {
    this.props.history.push("/insuranceplans");
    Swal.fire("Success", "Submitted Successfully", "success");
  };

  handleSubmitError = () => {
    _logger("error");
    Swal.fire("Error", "Something went wrong", "error");
  };

  handleUpdateSuccess = () => {
    this.setState(() => {
      return {
        isEditting: false
      };
    });
    this.props.history.push("/insuranceplans");
    Swal.fire("Success", "Updated Successfully", "success");
  };

  handleUpdateError = () => {
    _logger("error");
    Swal.fire("Error", "Something went wrong", "error");
  };

  handleCancel = () => {
    this.props.history.push(`/insuranceplans`);
  };

  render() {
    return (
      <React.Fragment>
        <Formik
          enableReinitialize={true}
          initialValues={this.state.formData}
          onSubmit={this.handleSubmit}
          validationSchema={insurancePlansSchema.insurancePlansValidate}
        >
          {props => {
            const {
              touched,
              errors,
              handleSubmit,
              isValid,
              isSubmitting
            } = props;
            return (
              <div
                style={{ padding: 0 }}
                className="card col-sm-12 col-md-6 col-lg-6 offset-3"
              >
                <div className="bg-light border-bottom p-3 mb-0">
                  New Insurance Plan
                </div>
                <div className="card-body">
                  <Form onSubmit={handleSubmit}>
                    <FormGroup>
                      <Label>Name</Label>
                      <Field
                        name="name"
                        type="text"
                        placeholder="Name"
                        autoComplete="off"
                        className={
                          errors.name && touched.name
                            ? "form-control error"
                            : "form-control"
                        }
                      />
                    </FormGroup>

                    <FormGroup>
                      <Label>Code</Label>
                      <Field
                        name="code"
                        type="text"
                        placeholder="Code"
                        autoComplete="off"
                        className={
                          errors.code && touched.code
                            ? "form-control error"
                            : "form-control"
                        }
                      />
                    </FormGroup>
                    <FormGroup>
                      <Label>Plan Providers</Label>
                      <Field
                        name="insuranceProviderId"
                        component="select"
                        label="planproviders"
                        className={
                          errors.insuranceProviderId &&
                          touched.insuranceProviderId
                            ? "form-control error"
                            : "form-control"
                        }
                        as="select"
                      >
                        <option>Select Status</option>
                        {this.state.plansProvider.map((ele, i) => (
                          <option key={i} value={ele.id}>
                            {ele.name}
                          </option>
                        ))}
                      </Field>
                    </FormGroup>
                    <FormGroup>
                      <Label>Plan Type</Label>
                      <Field
                        name="planTypeId"
                        component="select"
                        label="plantype"
                        className={
                          errors.planType && touched.planType
                            ? "form-control error"
                            : "form-control"
                        }
                        as="select"
                      >
                        <option>Select Status</option>
                        {this.state.plansType.map((ele, i) => (
                          <option key={i} value={ele.id}>
                            {ele.name}
                          </option>
                        ))}
                      </Field>
                    </FormGroup>
                    <FormGroup>
                      <Label>Plan Status</Label>
                      <Field
                        name="planStatusId"
                        component="select"
                        label="planstatus"
                        className={
                          errors.planStatus && touched.planStatus
                            ? "form-control error"
                            : "form-control"
                        }
                        as="select"
                      >
                        <option>Select Status</option>
                        {this.state.plansStatus.map((ele, i) => (
                          <option key={i} value={ele.id}>
                            {ele.name}
                          </option>
                        ))}
                      </Field>
                    </FormGroup>
                    <FormGroup>
                      <Label align="center" className="col">
                        Age (Min-Max)
                      </Label>
                      <div className="row">
                        <div className="col-1">
                          <Field
                            name="minAge"
                            type="number"
                            placeholder="Min Age"
                            autoComplete="off"
                            className={
                              errors.minAge && touched.minAge
                                ? "form-control error"
                                : "form-control"
                            }
                          />
                        </div>
                        <div className="col-1">
                          <Field
                            name="maxAge"
                            type="number"
                            placeholder="Max Age"
                            autoComplete="off"
                            className={
                              errors.maxAge && touched.maxAge
                                ? "form-control error"
                                : "form-control"
                            }
                          />
                        </div>
                      </div>
                    </FormGroup>
                    <div className="border-top pt-3 mt-3">
                      <Button
                        type="submit"
                        className="btn btn-success mr-2 btn btn-secondary"
                        disabled={!isValid || isSubmitting}
                      >
                        {this.state.isEditting ? "Edit" : "Submit"}
                      </Button>
                      <Button
                        type="button"
                        className="btn btn-dark btn btn-secondary"
                        onClick={this.handleCancel}
                      >
                        Cancel
                      </Button>
                    </div>
                  </Form>
                </div>
              </div>
            );
          }}
        </Formik>
      </React.Fragment>
    );
  }
}

InsurancePlansForm.propTypes = {
  match: propTypes.shape({
    params: propTypes.shape({
      id: propTypes.string
    })
  }),
  history: propTypes.shape({
    push: propTypes.func
  })
};
export default InsurancePlansForm;
