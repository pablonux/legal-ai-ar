import React from 'react';
import ErrorPage from './ErrorPage';

export default function ErrorPage500() {
    // const onClickLink = (event: React.MouseEvent<HTMLElement, MouseEvent>) => {
    //     console.log('500');
    // }
    // linkHref="https://www.google.com"
  return (
    <><ErrorPage errorImage={require("./svg/error-500.svg").default} pageTitle="Page under maintenance"></ErrorPage></>
  )
}
