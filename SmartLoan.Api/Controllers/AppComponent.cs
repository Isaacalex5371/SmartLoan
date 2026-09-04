import {
    Component, inject
}
from '@angular/core';
import {
    RouterLink, RouterLinkActive, RouterOutlet
}
from '@angular/router';
import {
    AuthStore
}
from './core/state/auth.store';
import {
    NotificationService
}
from './core/services/notification.service';
import {
    CommonModule
}
from '@angular/common';

@Component({
    selector:
    'app-root',
    standalone:
    true,
    imports:
    [RouterOutlet, RouterLink, RouterLinkActive, CommonModule],
    templateUrl:
    './app.component.html'
})
export

class AppComponent
{
    public readonly store = inject(AuthStore);
    public readonly notify = inject(NotificationService);

    logout()
    {
        if (confirm('Sign out of SmartLoan?'))
        {
            this.store.logout();
        }
    }
}