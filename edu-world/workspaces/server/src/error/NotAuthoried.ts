import httpStatus from "http-status";

class NotAuthorized extends Error {
    statusCode: number = httpStatus.UNAUTHORIZED;
    constructor(message: string) {
        super(message);
    }
}

export default NotAuthorized;