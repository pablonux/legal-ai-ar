import React from 'react';
import ErrorPage from './ErrorPage';

export default function Error404() {
    // const onClickLink = (event: React.MouseEvent<HTMLElement, MouseEvent>) => {
    //     console.log('404');
    // }
  // linkHref="https://www.google.com"
  return (
    <><ErrorPage errorImage={require("./svg/error-404.svg").default}></ErrorPage></>
  );
}