import React from "react";
import NonCompliantRow from "./NonCompliantRow";
import Pagination from "rc-pagination";
import localeInfo from "rc-pagination/lib/locale/en_US";
import * as providersService from "../../services/providersService";
import "rc-pagination/assets/index.css";
import Swal from "sweetalert2";

/* import logger from "sabio-debug";
const _logger = logger.extend("ProviderNonCompliant"); */

class ProviderNonCompliant extends React.Component {
  state = {
    providerNonCompliant: [],
    nonCompliantComponents: [],
    current: 1,
    pageSize: 10,
    total: 0,
    emailsArr: [],
    selectAllBoxes: false
  };

  componentDidMount() {
    this.setupPage();
  }

  shouldComponentUpdate(nextProps, nextState) {
    return (
      nextState.providerNonCompliant !== this.state.providerNonCompliant ||
      nextState.nonCompliantComponents !== this.state.nonCompliantComponents ||
      nextState.selectAllBoxes !== this.state.selectAllBoxes ||
      nextState.emailsArr !== this.state.emailsArr
    );
  }

  setupPage = () => {
    this.getAll(this.state.current - 1);
  };

  getAll = pageIndex => {
    providersService
      .getAllNonCompliant(pageIndex, this.state.pageSize)
      .then(this.onGetAllSuccess)
      .catch(this.onGetAllError);
  };

  onGetAllSuccess = res => {
    this.setState({
      providerNonCompliant: res.item.pagedItems,
      nonCompliantComponents: res.item.pagedItems.map(
        this.mapNonCompliantProvider
      ),
      total: res.item.totalCount,
      emailsArr: [],
      selectAllBoxes: false
    });
  };

  emailSelected = email => {
    return this.state.emailsArr.indexOf(email);
  };

  mapNonCompliantProvider = provider => {
    return (
      <NonCompliantRow
        key={provider.id}
        providerNonCompliant={provider}
        onCheckBoxClick={this.onCheckBoxClick}
        isChecked={this.emailSelected(provider.email) !== -1}
      />
    );
  };

  onCheckBoxClick = e => {
    let email = e.target.id;
    let emails = this.state.emailsArr;

    if (e.target.checked) {
      emails.push(email);
    } else {
      const index = emails.indexOf(email);
      emails.splice(index, 1);
    }

    this.setState(prevState => {
      return {
        emailsArr: emails,
        selectAllBoxes:
          emails.length === prevState.nonCompliantComponents.length,
        nonCompliantComponents: prevState.providerNonCompliant.map(
          this.mapNonCompliantProvider
        )
      };
    });
  };

  selectAllToggle = () => {
    this.setState(prevState => {
      const allSelected =
        prevState.emailsArr.length === prevState.providerNonCompliant.length;

      let emailsArr = [];
      if (allSelected === false) {
        emailsArr = prevState.providerNonCompliant.map(provider => {
          return provider.email;
        });
      }

      return {
        emailsArr,
        selectAllBoxes: !allSelected
      };
    }, this.updateCompliantComponents);
  };

  updateCompliantComponents = () => {
    this.setState(prevState => {
      return {
        nonCompliantComponents: prevState.providerNonCompliant.map(
          this.mapNonCompliantProvider
        )
      };
    });
  };

  changePage = current => {
    this.setState({ current }, this.setupPage);
  };

  sendEmail = () => {
    if (this.state.emailsArr.length > 0) {
      providersService
        .sendNonCompliantEmails(this.state.emailsArr)
        .then(this.onEmailSuccess)
        .catch(this.onEmailError);
    } else {
      Swal.fire("Error!", "Oh no! make a selection", "error");
    }
  };

  onEmailSuccess = () => {
    Swal.fire("Successful!", "Email Sent.", "success");
    this.setState({
      emailsArr: []
    });
  };

  renderHeaders = () => {
    const headers = [
      "Select",
      "Title",
      "First Name",
      "Mi",
      "Last Name",
      "Gender",
      "Phone",
      "Fax",
      "Email"
    ];
    return headers.map(this.mapHeader);
  };

  mapHeader = headerName => {
    return (
      <th className="p-1" key={"Header_" + headerName}>
        {headerName}
      </th>
    );
  };

  render() {
    return (
      <div className="col-sm-12">
        <div className="card">
          <div
            className="m-0 p-3 border-bottom bg-light card-title"
            style={{ fontWeight: "500", width: "100%" }}
          >
            Provider NonCompliant
          </div>
          <div className="card-body">
            <div className="react-bs-table-tool-bar">
              <div className="row">
                <div className="col-sm-6">
                  <button
                    type="button"
                    className="btn btn-success react-bs-table-add-btn"
                    onClick={this.selectAllToggle}
                  >
                    Select All Toggle
                  </button>
                </div>
                <div className="col-sm-6">
                  <div className="row d-flex justify-content-end">
                    <button
                      type="button"
                      className="btn btn-success react-bs-table-add-btn"
                      onClick={this.sendEmail}
                    >
                      Send Email
                    </button>
                  </div>
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
                          <thead className="table-header-wrapper">
                            <tr>{this.renderHeaders()}</tr>
                          </thead>
                          <tbody>{this.state.nonCompliantComponents}</tbody>
                        </table>
                      </div>
                    </div>
                    <div className="row justify-content-center">
                      <Pagination
                        className="d-flex justify-content-center"
                        showLessItems
                        current={this.state.current}
                        total={this.state.total}
                        onChange={this.changePage}
                        pageSize={this.state.pageSize}
                        showTitle={false}
                        locale={localeInfo}
                      />
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    );
  }
}

export default ProviderNonCompliant;
