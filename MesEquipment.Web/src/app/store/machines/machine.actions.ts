import { createActionGroup, emptyProps, props } from '@ngrx/store';
import { Machine, MachineQuery, PagedResult } from '../../models/machine';

export const MachineActions = createActionGroup({
  source: 'Machine',
  events: {
    'Load Machines': props<{ query: MachineQuery }>(),
    'Load Machines Success': props<{ result: PagedResult<Machine> }>(),
    'Load Machines Failure': props<{ error: string }>(),
  },
});
