/*
 * Copyright (c) 2019-2023. Michael Pogrebinsky - Top Developer Academy
 * https://topdeveloperacademy.com
 * All rights reserved
 */

import com.sun.net.httpserver.HttpExchange;
import com.sun.net.httpserver.HttpHandler;
import com.sun.net.httpserver.HttpServer;

import java.io.IOException;
import java.io.OutputStream;
import java.net.InetSocketAddress;
import java.nio.file.Files;
import java.nio.file.Paths;
import java.util.concurrent.Executor;
import java.util.concurrent.Executors;

/**
 * Optimizing for Throughput Part 2 - HTTP server + Jmeter
 * https://www.udemy.com/java-multithreading-concurrency-performance-optimization
 */
public class ThroughputHttpServer {
    private static final String INPUT_FILE = "./resources/war_and_peace.txt";
    private static final int NUMBER_OF_THREADS = 8;
    // 서버 리로스 초기화
    public static void main(String[] args) throws IOException {
        String text = new String(Files.readAllBytes(Paths.get(INPUT_FILE)));
        startServer(text);
    }
    // 서버 시작
    // "서버"에 "포트 번호를 8000를 사용하는 소켓 어드레스"
    // "컨택스트"에 "핸들러" 
    // "스레드 풀과 스레드 수가 8제작"
    // ⭐ 서버에 스레드 풀 연결 일명 Executor 연결시키기 ⭐️ 내가 알고싶은게 바로 이거다!
    public static void startServer(String text) throws IOException {
        HttpServer server = HttpServer.create(new InetSocketAddress(8000), 0);
        server.createContext("/search", new WordCountHandler(text));
        Executor executor = Executors.newFixedThreadPool(NUMBER_OF_THREADS);
        server.setExecutor(executor);
        server.start();
    }

    private static class WordCountHandler implements HttpHandler {
        private String text;

        public WordCountHandler(String text) {
            this.text = text;
        }
        // http://127.0.0.1:8000/search?word=talk 접속시 실행
        // 핸들러의 역할은
        // 네트워크의 Application Layer 계층에서 Request 데이터를 처리하고, Response 데이터를 만든다.
        @Override
        public void handle(HttpExchange httpExchange) throws IOException {
            String query = httpExchange.getRequestURI().getQuery();
            String[] keyValue = query.split("=");
            String action = keyValue[0];
            String word = keyValue[1];
            if (!action.equals("word")) {
                httpExchange.sendResponseHeaders(400, 0);
                return;
            }

            long count = countWord(word);

            byte[] response = Long.toString(count).getBytes();
            httpExchange.sendResponseHeaders(200, response.length);
            OutputStream outputStream = httpExchange.getResponseBody();
            outputStream.write(response);
            outputStream.close();
        }

        private long countWord(String word) {
            long count = 0;
            int index = 0;
            while (index >= 0) {
                index = text.indexOf(word, index);

                if (index >= 0) {
                    count++;
                    index++;
                }
            }
            return count;
        }
    }
}
