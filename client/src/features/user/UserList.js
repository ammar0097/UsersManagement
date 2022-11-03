import React from "react";
import Table from "react-bootstrap/Table";
import { useSelector } from "react-redux";

const UserList = ({ isLoading,  }) => {

    const { users } = useSelector((state) => state.user);
  
  return (
    <Table striped bordered>
      <thead>
        <tr>
          <th>Email</th>
          <th>Username</th>
        </tr>
      </thead>
      <tbody>
        {users && users.map((user) => (
          <tr>
            <td>{user.userName}</td>
            <td>{user.email}</td>
          </tr>
        ))}
      </tbody>
    </Table>
  );
};

export default UserList;
