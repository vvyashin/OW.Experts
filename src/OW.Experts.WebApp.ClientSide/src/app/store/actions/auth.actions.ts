import { Action } from '@ngrx/store';

export enum AuthActionTypes {
    SignIn = '[Auth] SignIn',
    SignInSuccess = '[Auth] Sign In Success',
    SignInFailure = '[Auth] Sign In Failure'
}

export class SignIn implements Action {
    readonly type = AuthActionTypes.SignIn;
    constructor(public payload: any) {}
}

export class SignInSuccess implements Action {
    readonly type = AuthActionTypes.SignInSuccess;
    constructor(public payload: any) {}
}

export class SignInFailure implements Action {
    readonly type = AuthActionTypes.SignInFailure;
    constructor(public payload: any) {}
}

export type AllAuthActions =
    | SignIn
    | SignInSuccess
    | SignInFailure;
