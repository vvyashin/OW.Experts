import { AuthActionTypes, AllAuthActions } from '../actions/auth.actions';

export interface State {
  isAuthenticated: boolean;
}

export const initialState: State = {
  isAuthenticated: false
};

export function reducer(state = initialState, action: AllAuthActions): State {
  switch (action.type) {
    case AuthActionTypes.SignInSuccess: {
      return {
        ...state,
        isAuthenticated: true,
      };
    }
    case AuthActionTypes.SignInFailure: {
      return {
        ...state,
        isAuthenticated: false
      };
    }
  }
}
