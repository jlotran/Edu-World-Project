import liveServer from "live-server";
import express from "express";
import compression from "compression";

const app = express();
app.use(compression());
app.use(express.static("public"));

const params = {
    port: 3000,
    host: "0.0.0.0",
    root: ".",
    file: "index.html",
    open: true,
    middleware: [
        function (req, res, next) {
            if (req.url.endsWith(".gz")) {
                res.setHeader("Content-Encoding", "gzip");

                // Đặt Content-Type chính xác cho các file Unity WebGL
                if (req.url.endsWith(".js.gz")) {
                    res.setHeader("Content-Type", "application/javascript");
                } else if (req.url.endsWith(".wasm.gz")) {
                    res.setHeader("Content-Type", "application/wasm");
                } else if (req.url.endsWith(".data.gz")) {
                    res.setHeader("Content-Type", "application/octet-stream");
                }
            }

            // Bật CORS để tránh lỗi tải file từ domain khác
            res.setHeader("Access-Control-Allow-Origin", "*");
            res.setHeader("Access-Control-Allow-Methods", "GET, OPTIONS");
            res.setHeader("Access-Control-Allow-Headers", "Content-Type");

            next();
        }
    ]
};

// Khởi động live-server
liveServer.start(params);