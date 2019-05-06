import { Component, OnDestroy, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { AppState } from 'src/app/store/app.states';

@Component({
    selector: 'app-sign-in',
    templateUrl: './sign-in.component.html',
    styleUrls: ['./sign-in.component.scss']
  })
export class SignInComponent implements OnDestroy, OnInit {
    private alive = true;

    constructor(private store: Store<AppState>) { }

    ngOnInit(): void {
    }

    ngOnDestroy(): void {
        this.alive = false;
    }
}
