import { createContext, useContext } from "react";
import ActitivtyStore from "./activityStore";
import CommonStore from "./commonStore";

interface Store {
    activityStore: ActitivtyStore,
    commonStore: CommonStore;
}

export const store: Store = {
    activityStore: new ActitivtyStore(),
    commonStore: new CommonStore()
}

export const StoreContext = createContext(store);

export function useStore() {
    return useContext(StoreContext);
}