package com.example.web.util;

import org.springframework.beans.BeansException;
import org.springframework.context.ApplicationContext;
import org.springframework.context.ApplicationContextAware;
import org.springframework.stereotype.Service;


@Service
public class BeanUtil implements ApplicationContextAware {

    private static ApplicationContext ctx;
    @Override
    public void setApplicationContext(ApplicationContext arg0)throws BeansException {
        ctx = arg0;
    }

    public static<T> T getBean(String beanName,Class<T> requiredType) {
        if(ctx == null){
            throw new NullPointerException();
        }
        return ctx.getBean(beanName,requiredType);
    }




}