import { createContext, useContext } from "react";
import ActitivtyStore from "./activityStore";

interface Store {
    activityStore: ActitivtyStore
}

export const store: Store = {
    activityStore: new ActitivtyStore()
}

export const StoreContext = createContext(store);

export function useStore() {
    return useContext(StoreContext);
}