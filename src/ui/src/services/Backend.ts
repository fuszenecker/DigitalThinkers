import { injectable } from "inversify";
import "reflect-metadata";

import { GetStockResult } from "../models/GetStockResult";

@injectable()
export default class Backend {
    private baseURL: string = ""

    constructor() {
        if (!process?.env?.NODE_ENV || process?.env?.NODE_ENV === 'development') {
            this.baseURL = "http://localhost:5000"
        } else {
            this.baseURL = ""
        }
    }

    public async getCoins(): Promise<GetStockResult> {
        return await (await fetch(new Request(`${this.baseURL}/api/v1/stock`))).json()
    }
}