﻿var gulp = require('gulp');
var browserify = require('browserify');
//var reactify = require('reactify');
var source = require('vinyl-source-stream');
var concat = require('gulp-concat');
var babelify = require("babelify");

var config = {
    paths: {
        html: "./src/*.html",
        js: "./src/**/*.{js,jsx}",
        css: [
            "./node_modules/bootstrap/dist/css/bootstrap.min.css",
            "./node_modules/bootstrap/dist/css/bootstrap-theme.min.css"
        ],
        appJs: "./src/app.js",
        dist: "./dist"
    }
};

gulp.task('html', function () {
    gulp.src(config.paths.html)
        .pipe(gulp.dest(config.paths.dist));
});

gulp.task('js', function () {
    browserify(config.paths.appJs)
        .transform("babelify", {presets: ["es2015", "react"]})
        //.transform(reactify)
        .bundle()
        .on('error', console.error.bind(console))
        .pipe(source('app.bundle.js'))
        .pipe(gulp.dest(config.paths.dist + "/scripts"));
});

gulp.task('css', function () {
    gulp.src(config.paths.css)
        .pipe(concat('bundle.css'))
        .pipe(gulp.dest(config.paths.dist + "/css"));
});

//gulp.task('watch', function () {
//    gulp.watch(config.paths.html, ['html']);
//    gulp.watch(config.paths.js, ['js']);
//});

gulp.task('default', ['html', 'css', 'js']);