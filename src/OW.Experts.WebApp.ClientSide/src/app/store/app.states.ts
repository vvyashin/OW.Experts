import * as auth from './reducers/auth.reducers';

export interface AppState {
    authState: auth.State;
}

export const Reducers = {
    auth: auth.reducer
};
