import React from "react";
import logger from "sabio-debug";
import PropTypes from "prop-types";

const _logger = logger.extend("SearchBar");

class SearchBar extends React.Component {
  state = {
    search: ""
  };

  static defaultProps = {
    placeholder: "Search Contents"
  };

  changeHandler = e => {
    this.setState({ search: e.target.value }, () => {
      _logger(this.state.search);
      this.props.onChange(this.state.search);
    });
  };

  render() {
    return (
      <React.Fragment>
        <input
          type="text"
          className="form-control"
          placeholder={this.props.placeholder}
          value={this.state.search}
          onChange={this.changeHandler}
        />
      </React.Fragment>
    );
  }
}

SearchBar.propTypes = {
  onChange: PropTypes.func.isRequired,
  placeholder: PropTypes.string
};

export default SearchBar;
