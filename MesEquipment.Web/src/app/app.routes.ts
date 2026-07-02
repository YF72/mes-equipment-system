import { Routes } from '@angular/router';
import { MachineListComponent } from './pages/machine-list/machine-list';
import { Login } from './pages/login/login';
import { authGuard } from './guards/auth.guard';
import { guestGuard } from './guards/guest.guard';

export const routes: Routes = [
  { path: '', redirectTo: 'machines', pathMatch: 'full' },
  { path: 'login', component: Login, canActivate: [guestGuard] },
  {
    path: 'machines',
    canActivate: [authGuard],
    component: MachineListComponent,
  },
  { path: '**', redirectTo: 'machines' },
];
