import { printf, toConsole } from "./fable_modules/fable-library-js.4.29.0/String.js";
import { Record, Union } from "./fable_modules/fable-library-js.4.29.0/Types.js";
import { record_type, list_type, tuple_type, decimal_type, string_type, int32_type, union_type, anonRecord_type, option_type, class_type } from "./fable_modules/fable-library-js.4.29.0/Reflection.js";
import { create } from "./fable_modules/fable-library-js.4.29.0/Date.js";
import { op_Addition, fromParts } from "./fable_modules/fable-library-js.4.29.0/Decimal.js";
import Decimal from "./fable_modules/fable-library-js.4.29.0/Decimal.js";
import { sumBy, ofArray } from "./fable_modules/fable-library-js.4.29.0/List.js";

export const myFirstVariable = 1;

export const secondValue = 2;

export const theAnswer = myFirstVariable + secondValue;

toConsole(printf("%d"))(theAnswer);

export class ShippingStatus extends Union {
    constructor(tag, fields) {
        super();
        this.tag = tag;
        this.fields = fields;
    }
    cases() {
        return ["Fulfilled", "Outstanding"];
    }
}

export function ShippingStatus_$reflection() {
    return union_type("Sample.ShippingStatus", [], ShippingStatus, () => [[["Item", anonRecord_type(["FulfilledOn", class_type("System.DateTime")], ["PaidOn", option_type(class_type("System.DateTime"))])]], [["Item", anonRecord_type(["DueOn", class_type("System.DateTime")])]]]);
}

export class Order extends Record {
    constructor(Id, PlacedOn, Status, Items) {
        super();
        this.Id = (Id | 0);
        this.PlacedOn = PlacedOn;
        this.Status = Status;
        this.Items = Items;
    }
}

export function Order_$reflection() {
    return record_type("Sample.Order", [], Order, () => [["Id", int32_type], ["PlacedOn", class_type("System.DateTime")], ["Status", ShippingStatus_$reflection()], ["Items", list_type(tuple_type(string_type, decimal_type))]]);
}

export const order = new Order(123, create(2022, 10, 1), new ShippingStatus(1, [{
    DueOn: create(2022, 10, 3),
}]), ofArray([["F# in Action Book", fromParts(40, 0, 0, false, 0)], ["New Laptop", fromParts(50, 0, 0, false, 0)]]));

export function totalValue(order_1) {
    return sumBy((tuple) => tuple[1], order_1.Items, {
        GetZero: () => (new Decimal("0")),
        Add: op_Addition,
    });
}

toConsole(`Order ${order.Id} has a value of ${totalValue(order)} and was placed on ${order.PlacedOn}`);

