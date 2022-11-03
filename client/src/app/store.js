import { configureStore } from '@reduxjs/toolkit';
import  user  from '../features/user/userSlice';
import auth from '../features/auth/authSlice'


export const store = configureStore({
  reducer: {
    user ,
    auth
  },
});
