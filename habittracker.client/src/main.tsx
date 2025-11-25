import { createRoot } from 'react-dom/client'
import './index.css'
import { App } from './App'
import { BrowserRouter } from 'react-router-dom'
import 'reflect-metadata'
import { AuthenticationService } from './classes/AuthenticationService'
import { RegisterSingleton, RegisterResolver } from '@here-mobility/micro-di';
import { GlobalEventObserver } from './classes/GlobalEventObserver'
import { UserService } from './classes/UserService'
import { ServerEventHandler } from './classes/ServerEventHandler'
import { HabitService } from './classes/HabitService'
import { HabitLogService } from './classes/HabitLogService'
import { HttpService } from './classes/HttpService'
import { AuthedHttpService } from './classes/AuthedHttpService'

const {VITE_BACKEND_URL} = import.meta.env;

const httpService = new HttpService();
const authedHttpService = new AuthedHttpService(httpService);

RegisterSingleton(AuthenticationService, () => new AuthenticationService(VITE_BACKEND_URL, httpService));
RegisterResolver(UserService, () => new UserService(VITE_BACKEND_URL, httpService, authedHttpService));
RegisterSingleton(GlobalEventObserver, () => {
  const observer = new GlobalEventObserver();

  //@ts-ignore
  window.observer = observer;
  return observer;
});
RegisterSingleton("IServerEventHandler", () => new ServerEventHandler(VITE_BACKEND_URL));
RegisterResolver(HabitService, () => new HabitService(VITE_BACKEND_URL, authedHttpService));
RegisterResolver(HabitLogService, () => new HabitLogService(VITE_BACKEND_URL, authedHttpService));
RegisterResolver(UserService, () => new UserService(VITE_BACKEND_URL, httpService, authedHttpService));

createRoot(document.getElementById('root')!).render(
  <BrowserRouter>
    <App />
  </BrowserRouter>
)
