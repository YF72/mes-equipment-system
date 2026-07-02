import { inject, Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { catchError, map, of, switchMap } from 'rxjs';
import { MachineService } from '../../services/machine.service';
import { MachineActions } from './machine.actions';

@Injectable()
export class MachineEffects {
  private actions$ = inject(Actions);
  private machineService = inject(MachineService);

  loadMachines$ = createEffect(() =>
    this.actions$.pipe(
      ofType(MachineActions.loadMachines),
      switchMap(({ query }) =>
        this.machineService.getMachines(query).pipe(
          map((result) => MachineActions.loadMachinesSuccess({ result })),
          catchError((error) => {
            console.error('Failed to load machines:', error);

            return of(
              MachineActions.loadMachinesFailure({
                error: 'Failed to load machines.',
              }),
            );
          }),
        ),
      ),
    ),
  );
}
