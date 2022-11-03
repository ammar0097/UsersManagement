import { createAsyncThunk, createSlice } from "@reduxjs/toolkit";
import axios from "axios";

export const register = createAsyncThunk(
  "auth/register",
  async (userData, thunkAPI) => {
    const { rejectWithValue } = thunkAPI;
    try {
      // configure header's Content-Type as JSON
      const config = {
        headers: {
          "Content-Type": "application/json",
        },
      };
      // make request to backend
      const { name, email, password } = userData;
      const { data } = await axios.post(
        "https://localhost:7231/api/Authentication/Register",
        { name, email, password },
        config
      );

      localStorage.setItem("token", data.token);
      localStorage.setItem("roles", data.roles);
      return data;
    } catch (error) {
      // return custom error message from API if any
      if (error.response && error.response.data.message) {
        return rejectWithValue(error.response.data.message);
      } else {
        return rejectWithValue(error.message);
      }
    }
  }
);


export const login = createAsyncThunk("auth/login",async (userData,thunkAPI) => {
  const {rejectWithValue} = thunkAPI;
  try {
     // configure header's Content-Type as JSON
     const config = {
      headers: {
        "Content-Type": "application/json",
      },
    };
    // make request to backend
    const { email, password } = userData;
    const { data } = await axios.post(
      "https://localhost:7231/api/Authentication/Register",
      { name, email, password },
      config
    );
    localStorage.setItem("token", data.token);
    localStorage.setItem("roles", data.roles);
  } catch (error) {
    
  }
})

export const authSlice = createSlice({
  name: "auth",
  initialState: {
    username: null,
    email: null,
    isLoading: false,
    isSuccess: false,
    isError: false,
    errorMessage: null,
  },
  reducers: {},
  extraReducers: {
    [register.pending]: (state, action) => {
      state.isLoading = true;
      state.isSuccess = false;
      state.isError = false;
      state.errorMessage = null;
    },
    [register.fulfilled]: (state, action) => {
      state.isLoading = false;
      state.errorMessage = null;
      state.isError = false;
      state.isSuccess = true;
      console.log(action.payload);
    },
    [register.rejected]: (state, action) => {
      state.isLoading = false;
      state.isError = true;
      state.isSuccess = false;
      state.errorMessage = null;
    },
  },
});

export default authSlice.reducer;
