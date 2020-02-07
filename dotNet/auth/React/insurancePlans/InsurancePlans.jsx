import React from "react";
import * as insurancePlansService from "../../services/insurancePlansService";
import logger from "sabio-debug";
import InsurancePlanCard from "./InsurancePlanCard";
import PropTypes from "prop-types";
import Pagination from "rc-pagination";
import SearchBar from "../SearchBar";
import "rc-pagination/assets/index.css";
import Swal from "sweetalert2";

const _logger = logger.extend("InsurancePlans");

class InsurancePlans extends React.Component {
  state = {
    planDetails: {},
    insurancePlans: [],
    insurancePlansData: [],
    pageIndex: 0,
    pageSize: 0,
    totalCount: 1,
    activePage: 1,
    search: ""
  };

  componentDidMount() {
    _logger("mounting");
    this.getAllInsurancePlans();
  }

  getAllInsurancePlans = () => {
    insurancePlansService
      .getAll(0, 12)
      .then(this.onGetAllSucess)
      .catch(this.onGetAllError);
  };

  onGetAllSucess = ({ item }) => {
    let insurancePlansData = item.pagedItems;
    let insurancePlans = insurancePlansData.map(this.mapInsurancePlans);
    this.setState(() => {
      return {
        insurancePlans,
        insurancePlansData,
        pageIndex: item.pageIndex,
        pageSize: item.pageSize,
        totalCount: item.totalCount
      };
    });
  };

  onGetAllError = () => {
    _logger("get all error");
  };

  handleSearchChange = searchWord => {
    this.setState(
      () => {
        return {
          search: searchWord
        };
      },
      () => {
        this.state.search.length > 0
          ? this.getSearchPagination(this.state.pageIndex)
          : this.getAllInsurancePlans();
      }
    );
  };

  getSearchPagination = page => {
    insurancePlansService
      .search(page, 12, this.state.search)
      .then(this.handleSearchChangeSuccess)
      .catch(this.handleSearchChangeError);
  };

  handleSearchChangeSuccess = res => {
    this.setState(() => {
      return {
        insurancePlans: res.item.pagedItems.map(this.mapInsurancePlans),
        insurancePlansData: res.item.pagedItems,
        pageIndex: res.item.pageIndex,
        pageSize: res.item.pageSize,
        totalCount: res.item.totalCount
      };
    });
  };

  handleSearchChangeError = () => {
    _logger("error on handle search change");
    this.setState({ ...this.state, insurancePlans: null });
  };
  deleteById = id => {
    insurancePlansService
      .deleteById(id)
      .then(this.deleteByIdSucess)
      .catch(this.deleteByIdError);
  };
  deleteByIdSucess = id => {
    this.setState(prevState => {
      let index = prevState.insurancePlansData.findIndex(
        plan => plan.id === id
      );
      let updatedPlans = [...prevState.insurancePlansData];
      updatedPlans.splice(index, 1);
      return {
        insurancePlans: updatedPlans.map(this.mapInsurancePlans),
        insurancePlansData: updatedPlans
      };
    });
    Swal.fire("Success", "Deleted Successfully", "success");
  };

  deleteByIdError = () => {
    _logger("error on delete by id");
    Swal.fire("Error", "Error on Delete", "error");
  };

  mapInsurancePlans = insurancePlans => {
    return (
      <InsurancePlanCard
        currentUserRoles={this.props.currentUser.roles}
        insurancePlans={insurancePlans}
        key={insurancePlans.id}
        handleEdit={this.handleEdit}
        handleDelete={this.handleDelete}
      />
    );
  };

  handleAddNew = () => {
    this.props.history.push(`/insuranceplans/new`);
  };

  handleDelete = id => {
    Swal.fire({
      title: "Are you sure?",
      text: "Confirm",
      type: "warning",
      showCancelButton: true,
      confirmButtonColor: "#DD6B55",
      confirmButtonText: "Yes,delete it!",
      cancelButtonText: "No,cancel please!",
      closeOnConfirm: false,
      closeOnCancel: false
    }).then(result => {
      if (result.value) {
        this.deleteById(id);
      } else if (result.dismiss === Swal.DismissReason.cancel) {
        Swal.fire("Cancelled", "Cancelled", "error");
      }
    });
  };

  handleEdit = insurancePlans => {
    this.props.history.push(`/insuranceplans/${insurancePlans.id}/edit`);
  };
  chooseHandlePageChange = pageNumber => {
    if (this.state.search) {
      this.getSearchPagination(pageNumber - 1);
    } else {
      this.handlePageChange(pageNumber - 1);
    }
  };
  handlePageChange = page => {
    insurancePlansService
      .getAll(page, 12)
      .then(this.handlePageChangeSuccess)
      .catch(this.handlePageChangeError);
  };

  handlePageChangeSuccess = ({ item }) => {
    this.setState(() => {
      return {
        insurancePlans: item.pagedItems.map(this.mapInsurancePlans),
        insurancePlansData: item.pagedItems,
        pageIndex: item.pageIndex,
        pageSize: item.pageSize,
        totalCount: item.totalCount,
        activePage: item.pageIndex + 1
      };
    });
  };

  handlePageChangeError = () => {
    _logger("error pagination");
  };

  renderAddButton = () => {
    return this.props.currentUser.roles.includes("SysAdmin") ? (
      <div className="col-sm-6 btn-group btn-group-sm">
        <button
          type="button"
          className="btn btn-success react-bs-table-add-btn "
          onClick={this.handleAddNew}
        >
          <span>
            <i className="fa glyphicon glyphicon-plus fa-plus text-white" /> New
          </span>
        </button>
      </div>
    ) : (
      ""
    );
  };

  renderHeaderColumns = () => {
    return this.props.currentUser.roles.includes("SysAdmin") ? (
      <thead className="table-header-wrapper">
        <tr>
          <th className="p-1">Plan Name</th>
          <th className="p-1">Code</th>
          <th className="p-1">Insurance Plan Level</th>
          <th className="p-1">Insurance Plan Type</th>
          <th className="p-1">Insurance Plan Code</th>
          <th className="p-1">Insurance Provider Name</th>
          <th className="p-1">Insurance Provider Website</th>
          <th className="p-1">Insurance Status</th>
          <th className="p-1">Edit</th>
          <th className="p-1">Delete</th>
        </tr>
      </thead>
    ) : (
      <thead className="table-header-wrapper">
        <tr>
          <th className="p-1">Plan Name</th>
          <th className="p-1">Code</th>
          <th className="p-1">Insurance Plan Level</th>
          <th className="p-1">Insurance Plan Type</th>
          <th className="p-1">Insurance Plan Code</th>
          <th className="p-1">Insurance Provider Name</th>
          <th className="p-1">Insurance Provider Website</th>
          <th className="p-1">Insurance Status</th>
        </tr>
      </thead>
    );
  };

  render() {
    return (
      <React.Fragment>
        <div className="col-sm-12">
          <div className="card card-hover">
            <div
              className="m-0 p-3 border-bottom bg-light card-title"
              style={{ fontWeight: "500", width: "100%" }}
            >
              Insurance Plans
            </div>
            <div className="card-body">
              <div className="react-bs-table-tool-bar">
                <div className="row">
                  {this.props.currentUser.roles.includes("SysAdmin")
                    ? this.renderAddButton()
                    : ""}
                  <div className="col-sm-6">
                    <SearchBar onChange={this.handleSearchChange} />
                  </div>
                </div>
              </div>
              <div className="tab-content mt-3 text-center">
                <div className="tab-pane active">
                  <div className="row">
                    <div className="col-sm-12">
                      <div>
                        <div className="table-responsive">
                          <table className="v-middle table table-striped table-bordered table-hover">
                            {this.renderHeaderColumns()}
                            <tbody>
                              {this.state.insurancePlansData.map(
                                this.mapInsurancePlans
                              )}
                            </tbody>
                          </table>
                        </div>
                      </div>
                      <div className="row justify-content-center">
                        <Pagination
                          defaultCurrent={this.state.activePage}
                          total={this.state.totalCount}
                          onChange={this.chooseHandlePageChange}
                          pageSize={this.state.pageSize}
                        />
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </React.Fragment>
    );
  }
}

InsurancePlans.propTypes = {
  history: PropTypes.shape({
    push: PropTypes.func
  }),
  prevPath: PropTypes.string,
  currentUser: PropTypes.shape({
    roles: PropTypes.arrayOf(PropTypes.string)
  })
};

export default InsurancePlans;
