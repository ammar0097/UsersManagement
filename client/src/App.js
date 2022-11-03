import React from "react";
import "./App.css";
import RegistrationForm from "./features/auth/RegistrationForm";
import UserContainer from "./features/user/UserContainer";
import Container from "react-bootstrap/Container";
import Row from "react-bootstrap/Row";
import Col from "react-bootstrap/Col";

function App() {
  return (
    <div className="App">
      <Container fluid>
        <Row>
          <Col>
            <RegistrationForm />
          </Col>
        </Row>
      </Container>
    </div>
  );
}

export default App;
