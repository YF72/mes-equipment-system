import { ApplicationConfig, isDevMode, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideStoreDevtools } from '@ngrx/store-devtools';
import { routes } from './app.routes';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { authInterceptor } from './services/auth.interceptor';
import { provideStore } from '@ngrx/store';
import { machineFeatureKey, machineReducer } from './store/machines/machine.reducer';
import { provideEffects } from '@ngrx/effects';
import { MachineEffects } from './store/machines/machine.effects';
import { API_BASE_URL, AuthClient, MachinesClient } from './api/mes-equipment-api';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes),
    provideHttpClient(withInterceptors([authInterceptor])),
    provideStore({
      [machineFeatureKey]: machineReducer,
    }),
    provideEffects([MachineEffects]),
    provideStoreDevtools({
      maxAge: 25,
      logOnly: !isDevMode(),
    }),
    { provide: API_BASE_URL, useValue: 'http://localhost:5264' },
    AuthClient,
    MachinesClient,
  ],
};
