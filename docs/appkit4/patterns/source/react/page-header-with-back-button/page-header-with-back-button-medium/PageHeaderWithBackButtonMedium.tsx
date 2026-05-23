import { useRef, useState } from 'react';
import PageHeaderWithBackButton from './PageHeaderWithBackButton';

export default function PageHeaderWithBackButtonMedium() {
  const breadcrumbArray = [{
    name: 'Product',
    link: 'Product'
  }, {
    name: 'Parent page',
    link: 'Parent'
  }, {
    name: 'This page',
    link: 'This'
  }];
  const [disabled, setDisabled] = useState(false);
  const [breadcrumb, setBreadcrumb] = useState(breadcrumbArray);
  const linkRef = useRef(2);
  const clickEvent = () => {
    if (linkRef.current > 0) {
      linkRef.current--;
      linkEvent(linkRef.current);
    }
    if (linkRef.current === 0) setDisabled(true);
  }
  const onClickBreadCrumbItem = (index: number) => {
    linkRef.current = index;
    switch (index) {
      case 0:
        setBreadcrumb([breadcrumbArray[0]]);
        setDisabled(true);
        break;
      case 1:
        setBreadcrumb([breadcrumbArray[0], breadcrumbArray[1]]);
        break;
      case 2:
        setBreadcrumb([breadcrumbArray[0], breadcrumbArray[1], breadcrumbArray[2]]);
        break;
      default:
        break;
    }
  }
  const linkEvent = (index: number) => {
    const start = `${window.location.protocol}//${window.location.host}`;
    switch (index) {
      case 0:
        setBreadcrumb([breadcrumbArray[0]]);
        // window.history.pushState('', '', start + '/' + breadcrumbArray[0].name);
        break;
      case 1:
        setBreadcrumb([breadcrumbArray[0], breadcrumbArray[1]]);
        // window.history.pushState('', '', start + '/' + breadcrumbArray[0].name + '/' + breadcrumbArray[1].name);
        break;
      case 2:
        setBreadcrumb([breadcrumbArray[0], breadcrumbArray[1], breadcrumbArray[2]]);
        break;
      default:
        break;
    }
  }

  return (
    <>
      <PageHeaderWithBackButton type="medium" breadcrumbList={breadcrumb} onClickEvent={clickEvent} onClickBreadCrumbItem={onClickBreadCrumbItem} disabledBack={disabled} tooltipContent="Back"></PageHeaderWithBackButton>
    </>
  )
}