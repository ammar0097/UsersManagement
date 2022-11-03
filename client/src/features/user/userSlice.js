import { createAsyncThunk, createSlice } from "@reduxjs/toolkit";


export const getUsers = createAsyncThunk(
  "user/getUsers",
  async (_, thunkAPI) => {
    const {rejectWithValue} = thunkAPI;
    try {
      const res = await fetch("https://localhost:7231/api/Setup/GetAllUsers");
      const data = await res.json();
      return  data;
    } catch (error) {
      return rejectWithValue(error.message);
    }
  }
);

export const userSlice = createSlice({
  name: "user",
  initialState: {
    users: null,
    isLoading: false,
  },
  reducers: {},
  extraReducers: {
    [getUsers.pending]: (state, action) => {
      state.isLoading = true;
      console.log(action);
    },
    [getUsers.rejected]: (state, action) => {
      state.isLoading = false;
      console.log(action);
    },
    [getUsers.fulfilled]: (state, action) => {
      state.isLoading = false;
      state.users = action.payload;
    },
  },
});

export default userSlice.reducer;
