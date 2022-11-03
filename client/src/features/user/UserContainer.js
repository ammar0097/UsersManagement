import React, { Fragment, useEffect } from "react";
import UserInfo from "./UserInfo";
import UserList from "./UserList";
import { useDispatch, useSelector } from "react-redux";
import { getUsers } from "./userSlice";

const UserContainer = () => {
  const dispatch = useDispatch();
  const { users, isLoading } = useSelector((state) => state.user);

  useEffect(() => {
    dispatch(getUsers());
  }, [dispatch]);

  return (
    <Fragment>
      <div className="row">
        <div className="col">
          <UserList isLoading={isLoading} users={users} />
        </div>
        <div className="col">
          <UserInfo />
        </div>
      </div>
    </Fragment>
  );
};

export default UserContainer;
