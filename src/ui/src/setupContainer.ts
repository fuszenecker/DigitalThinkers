import { Container } from "inversify";

import Backend from "./services/Backend";

function setupContainer(): Container {
    const container = new Container();
    container.bind(Backend).toSelf();
    // container.bind(Bar).toSelf();
    return container;
}

export default setupContainer;